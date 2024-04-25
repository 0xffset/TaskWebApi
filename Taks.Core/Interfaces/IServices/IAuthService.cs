using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taks.Core.Entities.ViewModels;

namespace Taks.Core.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<ResponseViewModel<UserViewModel>> Login(string username, string password);
        Task LogOut();
    }
}
