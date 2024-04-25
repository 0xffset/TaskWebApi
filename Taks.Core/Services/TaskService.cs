using Taks.Core.Entities.ViewModels;
using Taks.Core.Interfaces.IMapper;
using Taks.Core.Interfaces.IRepositories;
using Taks.Core.Interfaces.IServices;
using Task = Taks.Core.Entities.General.Task;

namespace Taks.Core.Services
{
    public class TaskService : BaseService<Task, TaskViewModel>, ITaskService
    {
        private readonly IBaseMapper<Task, TaskViewModel> _taskViewModelMapper;
        private readonly IBaseMapper<TaskCreateViewModel, Task> _taskCreateMapper;
        private readonly IBaseMapper<TaskUpdateViewModel, Task> _taskUpdateMapper;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserContext _userContext;

        public TaskService(IBaseMapper<Task, TaskViewModel> taskViewModelMapper,
                           IBaseMapper<TaskCreateViewModel, Task> taskCreateMapper,
                           IBaseMapper<TaskUpdateViewModel, Task> taskUpdateMapper,
                           ITaskRepository taskRepository,
                           IUserContext userContext) : base(taskViewModelMapper, taskRepository)
        {
            _taskViewModelMapper = taskViewModelMapper;
            _taskCreateMapper = taskCreateMapper;
            _taskUpdateMapper = taskUpdateMapper;
            _taskRepository = taskRepository;
            _userContext = userContext;
        }

        public async Task<TaskViewModel> Create(TaskCreateViewModel model, CancellationToken cancellationToken)
        {
            //Mapping through AutoMapper
            Task entity = _taskCreateMapper.MapModel(model);
            entity.EntryDate = DateTime.Now;
            entity.EntryBy = Convert.ToInt32(_userContext.UserId);

            return _taskViewModelMapper.MapModel(await _taskRepository.Create(entity, cancellationToken));
        }

        public async System.Threading.Tasks.Task Delete(int id, CancellationToken cancellationToken)
        {
            Task entity = await _taskRepository.GetById(id, cancellationToken);
            await _taskRepository.Delete(entity, cancellationToken);
        }

        public async System.Threading.Tasks.Task Update(TaskUpdateViewModel model, CancellationToken cancellationToken)
        {
            Task existingData = await _taskRepository.GetById(model.Id, cancellationToken);

            //Mapping through AutoMapper
            _ = _taskUpdateMapper.MapModel(model, existingData);

            // Set additional properties or perform other logic as needed
            existingData.UpdatedDate = DateTime.Now;
            existingData.UpdatedBy = Convert.ToInt32(_userContext.UserId);

            await _taskRepository.Update(existingData, cancellationToken);
        }
    }
}
