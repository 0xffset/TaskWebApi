using System;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Taks.Core.Entities.General;
using Taks.Core.Entities.ViewModels;
using Taks.Core.Interfaces.IMapper;
using Taks.Core.Interfaces.IRepositories;
using Taks.Core.Interfaces.IServices;
using Taks.Core.Services;
using Task = Taks.Core.Entities.General.Task;

namespace Tasks.UnitTesting.API
{
    public class TaskControllerTest
    {

        private Mock<IBaseMapper<Taks.Core.Entities.General.Task, TaskViewModel>> _taskViewModelMapperMock;
        private Mock<IBaseMapper<TaskCreateViewModel, Taks.Core.Entities.General.Task>> _taskCreateMapperMock;
        private Mock<IBaseMapper<TaskUpdateViewModel, Taks.Core.Entities.General.Task>> _taskUpdateMapperMock;
        private Mock<ITaskRepository> _taskRepositoryMock;
        private Mock<IUserContext> _userContextMock;


        [SetUp]
        public void Setup()
        {
            _taskViewModelMapperMock = new Mock<IBaseMapper<Taks.Core.Entities.General.Task, TaskViewModel>>();
            _taskCreateMapperMock = new Mock<IBaseMapper<TaskCreateViewModel, Task>>();
            _taskUpdateMapperMock = new Mock<IBaseMapper<TaskUpdateViewModel, Taks.Core.Entities.General.Task>>();
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _userContextMock = new Mock<IUserContext>();
        }

        [Test]
        public async System.Threading.Tasks.Task Create_ValidTask()
        {
            TaskService taskService = new(
              _taskViewModelMapperMock.Object,
              _taskCreateMapperMock.Object,
              _taskUpdateMapperMock.Object,
              _taskRepositoryMock.Object,
              _userContextMock.Object);


            TaskCreateViewModel TaskCreateViewModel = new()
            {
                Title = "New Task",
                Description = "Description",
                DueTime = DateTime.Today.AddDays(30),
                Status = true,
                CategoryId = 1

            };

            TaskViewModel TaskViewModel = new()
            {
                Title = "New Task",
                Description = "Description",
                DueTime = DateTime.Today.AddDays(30),
                Status = true,
                CategoryId = 1
            };

           Task Task = new()
            {
                Title = "New Task",
                Description = "Description",
                DueTime = DateTime.Today.AddDays(30),
                Status = true,
                CategoryId = 1
            };
            _ = _taskCreateMapperMock.Setup(mapper => mapper.MapModel(TaskCreateViewModel))
                             .Returns(Task);

            _ = _taskRepositoryMock.Setup(repo => repo.Create(Task, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(Task);

            _ = _taskViewModelMapperMock.Setup(mapper => mapper.MapModel(Task))
                                       .Returns(TaskViewModel);

            // Act
            TaskViewModel result = await taskService.Create(TaskCreateViewModel, It.IsAny<CancellationToken>());

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.That(result.Title, Is.EqualTo(TaskCreateViewModel.Title));
        }

        [Test]
        public async System.Threading.Tasks.Task Create_InValidTask()
        {
            TaskService taskService = new(
              _taskViewModelMapperMock.Object,
              _taskCreateMapperMock.Object,
              _taskUpdateMapperMock.Object,
              _taskRepositoryMock.Object,
              _userContextMock.Object);


            TaskCreateViewModel TaskCreateViewModel = new()
            {
                Title = "New Task 1",
                Description = "Description",
                DueTime = DateTime.Today.AddDays(30),
                Status = true,
                CategoryId = 1

            };

            TaskViewModel TaskViewModel = new()
            {
                Title = "New Task 2",
                Description = "Description",
                DueTime = DateTime.Today.AddDays(30),
                Status = true,
                CategoryId = 1
            };

            Task Task = new()
            {
                Title = "New Task 3",
                Description = "Description",
                DueTime = DateTime.Today.AddDays(30),
                Status = true,
                CategoryId = 1
            };
            _ = _taskCreateMapperMock.Setup(mapper => mapper.MapModel(TaskCreateViewModel))
                             .Returns(Task);

            _ = _taskRepositoryMock.Setup(repo => repo.Create(Task, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(Task);

            _ = _taskViewModelMapperMock.Setup(mapper => mapper.MapModel(Task))
                                       .Returns(TaskViewModel);

            // Act
            TaskViewModel result = await taskService.Create(TaskCreateViewModel, It.IsAny<CancellationToken>());

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.That(result.Title, Is.EqualTo(TaskCreateViewModel.Title));
        }
    }
}

