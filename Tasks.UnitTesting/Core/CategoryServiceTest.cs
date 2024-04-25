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

namespace Tasks.UnitTesting.Core
{
	public class CategoryServiceTest
	{
        private Mock<IBaseMapper<TaskCategory, CategoryViewModel>> _categoryViewModelMapperMock;
        private Mock<IBaseMapper<CategoryCreateViewModel, TaskCategory>> _categoryCreateMapperMock;
        private Mock<IBaseMapper<CategoryUpdateViewModel, TaskCategory>> _categoryUpdateMapperMock;
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private Mock<IUserContext> _userContextMock;

        [SetUp]
        public void Setup()
        {
            _categoryViewModelMapperMock = new Mock<IBaseMapper<TaskCategory, CategoryViewModel>>();
            _categoryCreateMapperMock = new Mock<IBaseMapper<CategoryCreateViewModel, TaskCategory>>();
            _categoryUpdateMapperMock = new Mock<IBaseMapper<CategoryUpdateViewModel, TaskCategory>>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _userContextMock = new Mock<IUserContext>();
        }

        [Test]
        public async System.Threading.Tasks.Task Create_ValidCategory()
        {
            CategoryService taskService = new(
              _categoryViewModelMapperMock.Object,
              _categoryCreateMapperMock.Object,
              _categoryUpdateMapperMock.Object,
              _categoryRepositoryMock.Object,
              _userContextMock.Object);

            CategoryCreateViewModel newCategoryCreateViewModel = new()
            {
                Name = "Apples"

            };

            CategoryViewModel newCategoryViewModel = new()
            {
                Name = "Apples"

            };



            TaskCategory createdCategory = new()
            {
                Name = "Apples"

            };


            _ = _categoryCreateMapperMock.Setup(mapper => mapper.MapModel(newCategoryCreateViewModel))
                          .Returns(createdCategory);

            _ = _categoryRepositoryMock.Setup(repo => repo.Create(createdCategory, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(createdCategory);

            _ = _categoryViewModelMapperMock.Setup(mapper => mapper.MapModel(createdCategory))
                                       .Returns(newCategoryViewModel);

            // Act
            CategoryViewModel result = await taskService.Create(newCategoryCreateViewModel, It.IsAny<CancellationToken>());

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.That(result.Name, Is.EqualTo(newCategoryCreateViewModel.Name));
            // Additional assertions for other properties

        }

        public CategoryServiceTest()
		{

		}
	}
}

