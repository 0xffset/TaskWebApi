using Taks.Core.Interfaces.IServices;

namespace Taks.Core.Services
{
    public class UserContext : IUserContext
    {
        public string? UserId { get; set; }

    }
}
