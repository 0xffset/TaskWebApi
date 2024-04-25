using Microsoft.AspNetCore.Identity;
using Taks.Core.Entities.General;
using Taks.Core.Entities.ViewModels;
using Taks.Core.Interfaces.IRepositories;
using Task = System.Threading.Tasks.Task;

namespace Tasks.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthRepository(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ResponseViewModel<UserViewModel>> Login(string userName, string password)
        {
            User? user = await _userManager.FindByNameAsync(userName);
            if (user == null || !user.IsActive)
            {
                return new ResponseViewModel<UserViewModel>
                {
                    Success = false,
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

            return result.Succeeded
                ? new ResponseViewModel<UserViewModel>
                {
                    Success = true,
                    Data = new UserViewModel { Id = user.Id, UserName = user.UserName },
                }
                : new ResponseViewModel<UserViewModel>
                {
                    Success = false
                };
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();

        }
    }
}
