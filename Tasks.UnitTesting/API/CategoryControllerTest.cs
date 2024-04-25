using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Taks.Core.Entities.General;
using Taks.Core.Entities.ViewModels;
using Taks.Core.Interfaces.IMapper;
using Taks.Core.Interfaces.IRepositories;
using Taks.Core.Interfaces.IServices;
using Taks.Core.Services;
using Tasks.API.Controllers.V1;
using Task = System.Threading.Tasks.Task;

namespace Tasks.UnitTesting.API
{
    public class CategoryControllerTest
    {
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<ILogger<CategoryController>> _loggerMock;
        private CategoryController _categoryController;

        [SetUp]
        public void Setup()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
            _loggerMock = new Mock<ILogger<CategoryController>>();
            _categoryController = new CategoryController(_loggerMock.Object, _categoryServiceMock.Object, default);
        }

        [Test]
        public async Task GetReturnsListCategories()
        {
            var Categories = new List<CategoryViewModel>
            {
                new CategoryViewModel{ Id = 1},
                new CategoryViewModel {Id = 2 }
            };

            _categoryServiceMock.Setup(service => service.GetAll(It.IsAny<CancellationToken>()))
           .ReturnsAsync(Categories);

            // Act
            var result = await _categoryController.Get(It.IsAny<CancellationToken>());

            // Assert
            ClassicAssert.IsInstanceOf<OkObjectResult>(result);
            var okObjectResult = (OkObjectResult)result;
            ClassicAssert.NotNull(okObjectResult);

            var model = (IEnumerable<CategoryViewModel>)okObjectResult.Value;
            ClassicAssert.NotNull(model);
            ClassicAssert.That(model.Count(), Is.EqualTo(Categories.Count));
        }
    }
}
