using Taks.Core.Entities.ViewModels;

namespace Taks.Core.Interfaces.IServices
{
    public interface ICategoryService : IBaseService<CategoryViewModel>
    {
        Task<CategoryViewModel> Create(CategoryCreateViewModel model, CancellationToken cancellationToken);
        Task Update(CategoryUpdateViewModel model, CancellationToken cancellationToken);
        Task Delete(int id, CancellationToken cancellationToken);

        Task<bool> CategoryExistById(int id);

    }
}
