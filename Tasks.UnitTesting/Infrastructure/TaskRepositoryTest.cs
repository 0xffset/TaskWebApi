using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using Tasks.Infrastructure.Data;
using Tasks.Infrastructure.Repositories;
using Taks.Core.Entities.General;
namespace Tasks.UnitTesting.Infrastructure
{
    public class TaskRepositoryTest
    {
        private Mock<ApplicationDbContext> _dbContextMock;
        private TaskRepository _taskRepository;

        [SetUp]
        public void Setup()
        {
            _dbContextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _taskRepository = new TaskRepository(_dbContextMock.Object);
        }

        [Test]
        public async System.Threading.Tasks.Task AddAsyncValidCategoryReturnsAddedTask()
        {

            // Arrange
            var newTask = new Taks.Core.Entities.General.Task
            {
               Title = "New Task",
               Description = "This task is related to Entity Framework",
               DueTime = DateTime.Today.AddDays(30),
               Status = true
            };

            var taskDbSetMock = new Mock<DbSet<Taks.Core.Entities.General.Task>>();

            _dbContextMock.Setup(db => db.Set<Taks.Core.Entities.General.Task>())
                          .Returns(taskDbSetMock.Object);

            taskDbSetMock.Setup(dbSet => dbSet.AddAsync(newTask, default))
                            .ReturnsAsync((EntityEntry<Taks.Core.Entities.General.Task>)null);

            // Act
            var result = await _taskRepository.Create(newTask, It.IsAny<CancellationToken>());


            // Assert
            NUnit.Framework.Legacy.ClassicAssert.NotNull(result);
            NUnit.Framework.Legacy.ClassicAssert.That(result, Is.EqualTo(newTask));
        }

    }
}

