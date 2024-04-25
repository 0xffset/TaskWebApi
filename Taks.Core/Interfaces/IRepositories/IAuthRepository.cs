using Taks.Core.Entities.ViewModels;

namespace Taks.Core.Interfaces.IRepositories
{
    public interface IAuthRepository
    {
        Task<ResponseViewModel<UserViewModel>> Login(string userName, string password);
        Task Logout();
    }
}
