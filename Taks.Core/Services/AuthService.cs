using Taks.Core.Entities.ViewModels;
using Taks.Core.Interfaces.IRepositories;
using Taks.Core.Interfaces.IServices;

namespace Taks.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        public async Task<ResponseViewModel<UserViewModel>> Login(string username, string password)
        {
            ResponseViewModel<UserViewModel> result = await _authRepository.Login(username, password);
            return result.Success
                ? new ResponseViewModel<UserViewModel>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = result.Data
                }
                : new ResponseViewModel<UserViewModel>
                {
                    Success = false,
                    Message = "Login failed",
                    Error = new ErrorViewModel
                    {
                        Code = "LOGIN_ERROR",
                        Message = "Incorrect username or password. Please check your credentials and try again."
                    }
                };
        }

        public async Task LogOut()
        {
            await _authRepository.Logout();
        }
    }
}
