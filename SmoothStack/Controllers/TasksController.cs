using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmoothStack.Models.DTOs;
using SmoothStack.Services.Task;
using System.Security.Claims;

namespace SmoothStack.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] string? status, [FromQuery] int? assignee)
        {
            try
            {
                var tasks = await _taskService.GetTasksAsync(status, assignee);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tasks");
                return StatusCode(500, new { message = "Failed to fetch tasks" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);

                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching task {id}");
                return StatusCode(500, new { message = "Failed to fetch task" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var task = await _taskService.CreateTaskAsync(createDto, userId);

                _logger.LogInformation($"Task created: {task.Id}");
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, new { message = "Failed to create task" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var task = await _taskService.UpdateTaskAsync(id, updateDto, userId);

                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                _logger.LogInformation($"Task updated: {id}");
                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating task {id}");
                return StatusCode(500, new { message = "Failed to update task" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var deleted = await _taskService.DeleteTaskAsync(id, userId);

                if (!deleted)
                {
                    return NotFound(new { message = "Task not found or you don't have permission to delete it" });
                }

                _logger.LogInformation($"Task deleted: {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting task {id}");
                return StatusCode(500, new { message = "Failed to delete task" });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim?.Value ?? "0");
        }
    }
}
