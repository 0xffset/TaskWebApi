using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using Tasks.Infrastructure.Data;
using Tasks.Infrastructure.Repositories;
using Taks.Core.Entities.General;
using Task = System.Threading.Tasks.Task;

namespace Tasks.UnitTesting.Infrastructure
{
	public class CategoryRepositoryTest
	{
        private Mock<ApplicationDbContext> _dbContextMock;
        private CategoryRepository _categoryRepository;

        [SetUp]
        public void Setup()
        {
            _dbContextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _categoryRepository = new CategoryRepository(_dbContextMock.Object);
        }

        [Test]
        public async Task AddAsyncValidCategoryReturnsAddedCategory()
        {

            // Arrange
            var newCategory = new TaskCategory
            {
                Name = "Category 1"
            };

            var categoryDbSetMock = new Mock<DbSet<TaskCategory>>();

            _dbContextMock.Setup(db => db.Set<TaskCategory>())
                          .Returns(categoryDbSetMock.Object);

            categoryDbSetMock.Setup(dbSet => dbSet.AddAsync(newCategory, default))
                            .ReturnsAsync((EntityEntry<TaskCategory>)null);

            // Act
            var result = await _categoryRepository.Create(newCategory, It.IsAny<CancellationToken>());


            // Assert
            NUnit.Framework.Legacy.ClassicAssert.NotNull(result);
            NUnit.Framework.Legacy.ClassicAssert.That(result, Is.EqualTo(newCategory));
        }
     
	}
}

