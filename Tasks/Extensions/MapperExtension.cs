using AutoMapper;
using Taks.Core.Entities.General;
using Taks.Core.Entities.ViewModels;
using Taks.Core.Interfaces.IMapper;
using Taks.Core.Mapper;

namespace Tasks.API.Extensions
{
    public static class MapperExtension
    {
        public static IServiceCollection RegisterMapperService(this IServiceCollection services)
        {

            #region Mapper
            _ = services.AddSingleton<IMapper>(sp => new MapperConfiguration(cfg =>
            {

                _ = cfg.CreateMap<Taks.Core.Entities.General.Task, TaskViewModel>();
                _ = cfg.CreateMap<TaskCreateViewModel, Taks.Core.Entities.General.Task>();
                _ = cfg.CreateMap<TaskUpdateViewModel, Taks.Core.Entities.General.Task>();


                _ = cfg.CreateMap<TaskCategory, CategoryViewModel>();
                _ = cfg.CreateMap<CategoryCreateViewModel, TaskCategory>();
                _ = cfg.CreateMap<CategoryUpdateViewModel, TaskCategory>();

                _ = cfg.CreateMap<Role, RoleViewModel>();
                _ = cfg.CreateMap<RoleCreateViewModel, Role>();
                _ = cfg.CreateMap<RoleUpdateViewModel, Role>();

                _ = cfg.CreateMap<User, UserViewModel>();
                _ = cfg.CreateMap<UserViewModel, User>();
            }).CreateMapper());

            _ = services.AddSingleton<IBaseMapper<Taks.Core.Entities.General.Task, TaskViewModel>, BaseMapper<Taks.Core.Entities.General.Task, TaskViewModel>>();
            _ = services.AddSingleton<IBaseMapper<TaskCreateViewModel, Taks.Core.Entities.General.Task>, BaseMapper<TaskCreateViewModel, Taks.Core.Entities.General.Task>>();
            _ = services.AddSingleton<IBaseMapper<TaskUpdateViewModel, Taks.Core.Entities.General.Task>, BaseMapper<TaskUpdateViewModel, Taks.Core.Entities.General.Task>>();

            _ = services.AddSingleton<IBaseMapper<Role, RoleViewModel>, BaseMapper<Role, RoleViewModel>>();
            _ = services.AddSingleton<IBaseMapper<RoleCreateViewModel, Role>, BaseMapper<RoleCreateViewModel, Role>>();
            _ = services.AddSingleton<IBaseMapper<RoleUpdateViewModel, Role>, BaseMapper<RoleUpdateViewModel, Role>>();

            _ = services.AddSingleton<IBaseMapper<TaskCategory, CategoryViewModel>, BaseMapper<TaskCategory, CategoryViewModel>>();
            _ = services.AddSingleton<IBaseMapper<CategoryCreateViewModel, TaskCategory>, BaseMapper<CategoryCreateViewModel, TaskCategory>>();
            _ = services.AddSingleton<IBaseMapper<CategoryUpdateViewModel, TaskCategory>, BaseMapper<CategoryUpdateViewModel, TaskCategory>>();

            _ = services.AddSingleton<IBaseMapper<User, UserViewModel>, BaseMapper<User, UserViewModel>>();
            _ = services.AddSingleton<IBaseMapper<UserViewModel, User>, BaseMapper<UserViewModel, User>>();
            #endregion 
            return services;
        }
    }
}
