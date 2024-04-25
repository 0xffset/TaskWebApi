using Taks.Core.Entities.General;
using Taks.Core.Entities.ViewModels;
using Taks.Core.Interfaces.IMapper;
using Taks.Core.Interfaces.IRepositories;
using Taks.Core.Interfaces.IServices;

namespace Taks.Core.Services
{
    public class CategoryService : BaseService<TaskCategory, CategoryViewModel>, ICategoryService
    {
        private readonly IBaseMapper<TaskCategory, CategoryViewModel> _categoryViewModelMapper;
        private readonly IBaseMapper<CategoryCreateViewModel, TaskCategory> _categoryCreateMapper;
        private readonly IBaseMapper<CategoryUpdateViewModel, TaskCategory> _categoryUpdateMapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserContext _userContext;

        public CategoryService(IBaseMapper<TaskCategory, CategoryViewModel> categoryViewModelMapper,
                               IBaseMapper<CategoryCreateViewModel, TaskCategory> categoryCreateMapper,
                               IBaseMapper<CategoryUpdateViewModel, TaskCategory> categoryUpdateMapper,
                               ICategoryRepository categoryRepository,
                               IUserContext userContext) : base(categoryViewModelMapper, categoryRepository)
        {
            _categoryViewModelMapper = categoryViewModelMapper;
            _categoryCreateMapper = categoryCreateMapper;
            _categoryUpdateMapper = categoryUpdateMapper;
            _categoryRepository = categoryRepository;
            _userContext = userContext;
        }



        public async Task<CategoryViewModel> Create(CategoryCreateViewModel model, CancellationToken cancellationToken)
        {
            //Mapping through AutoMapper
            TaskCategory entity = _categoryCreateMapper.MapModel(model);
            entity.EntryDate = DateTime.Now;
            entity.EntryBy = Convert.ToInt32(_userContext.UserId);

            return _categoryViewModelMapper.MapModel(await _categoryRepository.Create(entity, cancellationToken));
        }

        public async System.Threading.Tasks.Task Delete(int id, CancellationToken cancellationToken)
        {
            TaskCategory entity = await _categoryRepository.GetById(id, cancellationToken);
            await _categoryRepository.Delete(entity, cancellationToken);
        }
        public async Task<bool> CategoryExistById(int id)
        {
            return await _categoryRepository.CategoryExistById(id);
        }


        public async System.Threading.Tasks.Task Update(CategoryUpdateViewModel model, CancellationToken cancellationToken)
        {
            TaskCategory existingData = await _categoryRepository.GetById(model.Id, cancellationToken);

            //Mapping through AutoMapper
            existingData.Name = model.Name;
            existingData.Status = model.Status;

            //_categoryViewModelMapper.MapModel(model, exi);

            // Set additional properties or perform other logic as needed
            existingData.UpdatedDate = DateTime.Now;
            existingData.UpdatedBy = Convert.ToInt32(_userContext.UserId);

            await _categoryRepository.Update(existingData, cancellationToken);
        }
    }
}
