using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Taks.Core.Common;
using Taks.Core.Entities.ViewModels;
using Taks.Core.Interfaces.IServices;
using Tasks.API.Helpers;

namespace Tasks.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;

        public AuthController(ILogger<AuthController> logger, IAuthService authService, IConfiguration configuration, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _authService = authService;
            _configuration = configuration;
            _appSettings = appSettings.Value;
        }
        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ResponseViewModel<UserViewModel> result = await _authService.Login(model.UserName, model.Password);
                    if (result.Success)
                    {
                        AuthResultViewModel token = GenerateJwtToken(result);
                        return Ok(new ResponseViewModel<AuthResultViewModel>
                        {
                            Success = true,
                            Data = token,
                            Message = "Login successful"
                        });
                    }

                    return BadRequest(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while login");
                    string message = $"An error occurred while login- " + ex.Message;

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "LOGIN_ERROR",
                            Message = message
                        }
                    });
                }

            }

            return BadRequest(new ResponseViewModel
            {
                Success = false,
                Message = "Invalid input",
                Error = new ErrorViewModel
                {
                    Code = "INPUT_VALIDATION_ERROR",
                    Message = ModalStateHelper.GetErrors(ModelState)
                }
            });
        }
        /// <summary>
        /// Logout the usesr
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogOut();
            return Ok();
        }
        private AuthResultViewModel GenerateJwtToken(ResponseViewModel<UserViewModel> auth)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_appSettings.JwtConfig.Secret);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Aud, _appSettings.JwtConfig.ValidAudience),
                new Claim(JwtRegisteredClaimNames.Iss, _appSettings.JwtConfig.ValidIssuer),
                new Claim(JwtRegisteredClaimNames.Sub, auth.Data.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_appSettings.JwtConfig.TokenExpirationMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = jwtTokenHandler.WriteToken(token);

            return new AuthResultViewModel()
            {
                AccessToken = jwtToken,
                Success = true,
            };
        }
    }
}
