using Taks.Core.Interfaces.IRepositories;
using Taks.Core.Interfaces.IServices;
using Taks.Core.Services;
using Tasks.Infrastructure.Repositories;

namespace Tasks.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            #region Services
            _ = services.AddSingleton<IUserContext, UserContext>();
            _ = services.AddScoped<IAuthService, AuthService>();
            _ = services.AddScoped<ICategoryService, CategoryService>();
            _ = services.AddScoped<ITaskService, TaskService>();
            #endregion

            #region Repositories
            _ = services.AddTransient<IAuthRepository, AuthRepository>();
            _ = services.AddTransient<ICategoryRepository, CategoryRepository>();
            _ = services.AddTransient<ITaskRepository, TaskRepository>();

            #endregion

            return services;
        }

    }
}
