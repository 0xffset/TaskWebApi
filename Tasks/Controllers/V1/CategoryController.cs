using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Taks.Core.Entities.ViewModels;
using Taks.Core.Interfaces.IServices;
using Tasks.API.Helpers;

namespace Tasks.API.Controllers.V1
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IMemoryCache _memoryCache;

        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _categoryService = categoryService;
            _memoryCache = memoryCache;
        }
        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /v1/Category
        ///     {
        ///        
        ///        "name": "Studies",
        ///       
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                string message;
                if (await _categoryService.IsExists("Name", model.Name, cancellationToken))
                {
                    message = $"The category name- '{model.Name}' already exists";
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel<CategoryViewModel>
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "DUPLICATE_CODE",
                            Message = message
                        }
                    });
                }

                try
                {
                    CategoryViewModel data = await _categoryService.Create(model, cancellationToken);

                    ResponseViewModel<CategoryViewModel> response = new()
                    {
                        Success = true,
                        Message = "Category created successfully",
                        Data = data
                    };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while adding the category");
                    message = $"An error occurred while adding the category- " + ex.Message;

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel<CategoryViewModel>
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "ADD_ROLE_ERROR",
                            Message = message
                        }
                    });
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel<CategoryViewModel>
            {
                Success = false,
                Message = "Invalid input",
                Error = new ErrorViewModel
                {
                    Code = "INPUT_VALIDATION_ERROR",
                    Message = ModalStateHelper.GetErrors(ModelState)
                }
            });
        }
        /// <summary>
        /// Updates a category by id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /v1/Category
        ///     {
        ///        "id": 1,
        ///        "name": "Studies",
        ///        "status": "true"
        ///     }
        ///
        /// </remarks>
        [HttpPut]
        public async Task<IActionResult> Edit(CategoryUpdateViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                string message;
                if (await _categoryService.IsExists("Name", model.Name, cancellationToken))
                {
                    message = $"The category name- '{model.Name}' already exists";
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "DUPLICATE_NAME",
                            Message = message
                        }
                    });
                }

                try
                {
                    await _categoryService.Update(model, cancellationToken);

                    // Remove data from cache by key
                    _memoryCache.Remove($"Category_{model.Id}");

                    ResponseViewModel response = new()
                    {
                        Success = true,
                        Message = "Category updated successfully"
                    };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the category");
                    message = $"An error occurred while updating the category- " + ex.Message;

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "UPDATE_ROLE_ERROR",
                            Message = message
                        }
                    });
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel
            {
                Success = false,
                Message = "Invalid input",
                Error = new ErrorViewModel
                {
                    Code = "INPUT_VALIDATION_ERROR",
                    Message = ModalStateHelper.GetErrors(ModelState)
                }
            });
        }
        /// <summary>
        /// Deletes a category by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /v1/Category/1
        ///
        /// </remarks>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _categoryService.Delete(id, cancellationToken);

                // Remove data from cache by key
                _memoryCache.Remove($"Category_{id}");

                ResponseViewModel response = new()
                {
                    Success = true,
                    Message = "Category deleted successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.Message == "No data found")
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseViewModel
                    {
                        Success = false,
                        Message = "Category not found",
                        Error = new ErrorViewModel
                        {
                            Code = "NOT_FOUND",
                            Message = "Category not found"
                        }
                    });
                }

                _logger.LogError(ex, "An error occurred while deleting the category");

                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel
                {
                    Success = false,
                    Message = "Error deleting the category",
                    Error = new ErrorViewModel
                    {
                        Code = "DELETE_ROLE_ERROR",
                        Message = ex.Message
                    }
                });

            }
        }
        /// <summary>
        /// Gets a category by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /v1/Category/1
        ///
        /// </remarks>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            try
            {
                CategoryViewModel? category = new();

                // Attempt to retrieve the categry from the cache
                if (_memoryCache.TryGetValue($"Category_{id}", out CategoryViewModel cachedCategory))
                {
                    category = cachedCategory;
                }
                else
                {
                    // If not found in cache, fetch the category from the data source
                    category = await _categoryService.GetById(id, cancellationToken);

                    if (category != null)
                    {
                        // Cache the category with an expiration time of 10 minutes
                        _ = _memoryCache.Set($"Category_{id}", category, TimeSpan.FromMinutes(10));
                    }
                }

                ResponseViewModel<CategoryViewModel> response = new()
                {
                    Success = true,
                    Message = "Category retrieved successfully",
                    Data = category
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.Message == "No data found")
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseViewModel<CategoryViewModel>
                    {
                        Success = false,
                        Message = "Category not found",
                        Error = new ErrorViewModel
                        {
                            Code = "NOT_FOUND",
                            Message = "Category not found"
                        }
                    });
                }

                _logger.LogError(ex, $"An error occurred while retrieving the category");

                ResponseViewModel<CategoryViewModel> errorResponse = new()
                {
                    Success = false,
                    Message = "Error retrieving category",
                    Error = new ErrorViewModel
                    {
                        Code = "ERROR_CODE",
                        Message = ex.Message
                    }
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
        /// <summary>
        /// Get all categories 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<CategoryViewModel> categories = await _categoryService.GetAll(cancellationToken);

                ResponseViewModel<IEnumerable<CategoryViewModel>> response = new()
                {
                    Success = true,
                    Message = "Categories retrieved successfully",
                    Data = categories
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories");

                ResponseViewModel<IEnumerable<CategoryViewModel>> errorResponse = new()
                {
                    Success = false,
                    Message = "Error retrieving categories",
                    Error = new ErrorViewModel
                    {
                        Code = "ERROR_CODE",
                        Message = ex.Message
                    }
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}
