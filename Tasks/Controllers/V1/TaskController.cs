using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Xml;
using Taks.Core.Entities.ViewModels;
using Taks.Core.Interfaces.IServices;
using Taks.Core.Services;
using Tasks.API.Helpers;

namespace Tasks.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
        private readonly ITaskService _taskService;
        private readonly ICategoryService _categoryService;
        private readonly IMemoryCache _memoryCache;

        public TaskController(ILogger<TaskController> logger, ITaskService taskService, ICategoryService categoryService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _taskService = taskService;
            _categoryService = categoryService;
            _memoryCache = memoryCache;
        }
        /// <summary>
        /// Creates a new task
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
        ///       "title": "Dinner",
        ///       "description": "Make the dinner tomorrow",
        ///       "status": "true",
        ///       "dueTime": "2024-04-25T14:49:42.463Z",
        ///       "categoryId": 2
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Create(TaskCreateViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                string message;

                if (!await _categoryService.CategoryExistById(model.CategoryId))
                {
                    message = $"The category does not exists.";
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel<CategoryViewModel>
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "INPUT_VALIDATION_ERROR",
                            Message = message
                        }
                    });
                }

                if (await _taskService.IsExists("Title", model.Title, cancellationToken))
                {
                    message = $"The category name- '{model.Title}' already exists";
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
                if (model.DueTime.Date <  DateTime.Now.Date)
                {
                    message = $"The due time must be greater that today.";
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel<CategoryViewModel>
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "DUE_TIME_ERROR",
                            Message = message
                        }
                    });
                }

                try
                {
                    TaskViewModel data = await _taskService.Create(model, cancellationToken);

                    ResponseViewModel<TaskViewModel> response = new()
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

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel<TaskViewModel>
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

            return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel<TaskViewModel>
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
        /// Updates a task by id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /v1/Category
        ///     {
        ///       "id": 1, 
        ///       "title": "Dinner",
        ///       "description": "Make the dinner tomorrow",
        ///       "status": "true",
        ///       "dueTime": "2024-04-25T14:49:42.463Z",
        ///       "categoryId": 2
        ///     }
        ///
        /// </remarks>

        [HttpPut]
        public async Task<IActionResult> Edit(TaskUpdateViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                string message;
                if (await _taskService.IsExists("Title", model.Title, cancellationToken))
                {
                    message = $"The task name- '{model.Title}' already exists";
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseViewModel
                    {
                        Success = false,
                        Message = message,
                        Error = new ErrorViewModel
                        {
                            Code = "DUPLICATE_TITLE",
                            Message = message
                        }
                    });
                }

                try
                {
                    await _taskService.Update(model, cancellationToken);

                    // Remove data from cache by key
                    _memoryCache.Remove($"Task_{model.Id}");

                    ResponseViewModel response = new()
                    {
                        Success = true,
                        Message = "Task updated successfully"
                    };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while updating the task");
                    message = $"An error occurred while updating the task- " + ex.Message;

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
        /// Deletes a task by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /v1/Task/1
        ///
        /// </remarks>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _taskService.Delete(id, cancellationToken);

                // Remove data from cache by key
                _memoryCache.Remove($"Task_{id}");

                ResponseViewModel response = new()
                {
                    Success = true,
                    Message = "Task deleted successfully"
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
                        Message = "Task not found",
                        Error = new ErrorViewModel
                        {
                            Code = "NOT_FOUND",
                            Message = "Task not found"
                        }
                    });
                }

                _logger.LogError(ex, "An error occurred while deleting the task");

                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseViewModel
                {
                    Success = false,
                    Message = "Error deleting the task",
                    Error = new ErrorViewModel
                    {
                        Code = "DELETE_ROLE_ERROR",
                        Message = ex.Message
                    }
                });

            }
        }
        /// <summary>
        /// Get a task by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /v1/Task/1
        ///
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                TaskViewModel? task = new TaskViewModel();

                // Attempt to retrieve the categry from the cache
                if (_memoryCache.TryGetValue($"Task_{id}", out TaskViewModel cachedCategory))
                {
                    task = cachedCategory;
                }
                else
                {
                    // If not found in cache, fetch the category from the data source
                    task = await _taskService.GetById(id, cancellationToken);

                   

                    if (task != null)
                    {
                        // Cache the category with an expiration time of 10 minutes
                        _ = _memoryCache.Set($"Task_{id}", task, TimeSpan.FromMinutes(10));
                    }
                }

                ResponseViewModel<TaskViewModel> response = new()
                {
                    Success = true,
                    Message = "Task retrieved successfully",
                    Data = task
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.Message == "No data found")
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseViewModel<TaskViewModel>
                    {
                        Success = false,
                        Message = "Task not found",
                        Error = new ErrorViewModel
                        {
                            Code = "NOT_FOUND",
                            Message = "Task not found"
                        }
                    });
                }

                _logger.LogError(ex, $"An error occurred while retrieving the task");

                ResponseViewModel<TaskViewModel> errorResponse = new()
                {
                    Success = false,
                    Message = "Error retrieving task",
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
        /// Get all tasks
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
               
                IEnumerable<TaskViewModel> tasks = await _taskService.GetAll(cancellationToken);

                ResponseViewModel<IEnumerable<TaskViewModel>> response = new()
                {
                    Success = true,
                    Message = "Tasks retrieved successfully",
                    Data = tasks
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving tasks");

                ResponseViewModel<IEnumerable<TaskViewModel>> errorResponse = new()
                {
                    Success = false,
                    Message = "Error retrieving tasks",
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
