using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.Services.Interfaces;
using System.Threading.Tasks;
using TaskManagerAPI.Models;
using TaskManagerAPI.Enums;
using TaskManagerAPI.Dtos;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("projects/{projectId}/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _tasksService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService tasksService,ILogger<TasksController> logger)
        {
            _logger = logger;
            _tasksService = tasksService;
        }
        // GET: projects/{projectId}/tasks
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTasks(int projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var tasks = await _tasksService.GetTasksAsync(User, projectId, page, pageSize);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Error retrieving tasks: {ex.Message}");
            }
        }
        // POST: projects/{projectId}/tasks
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTask(int projectId, [FromBody] TaskDto task)
        {
            try
            {
                var created = await _tasksService.AddTaskAsync(task, projectId, User);
                return Ok(created);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Error adding task: {ex.Message}");
            }
        }
        // PUT: projects/{projectId}/tasks/{taskId}
        [HttpPut("{taskId}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask(int projectId,int taskId, [FromBody] TaskDto task)
        {
            try
            {
                var updated = await _tasksService.UpdateTaskAsync(task, taskId,projectId, User);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(403, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Error updating task: {ex.Message}");
            }
        }
        // PATCH: projects/{projectId}/tasks/{taskId}/status
        [HttpPatch("{taskId}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateTaskStatus(int projectId,int taskId, [FromQuery] TaskItemStatus status)
        {
            try
            {
                var updated = await _tasksService.UpdateTaskStatusAsync(status, taskId,projectId, User);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(403, ex.Message); 
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Error updating task status: {ex.Message}");
            }
        }
        // DELETE: projects/{projectId}/tasks/{taskId}
        [HttpDelete("{taskId}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(int projectId,int taskId)
        {
            try
            {
                var deleted = await _tasksService.DeleteTaskAsync(taskId,projectId, User);
                if (!deleted)
                    return NotFound("Task not found");

                return Ok("Task deleted successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Error deleting task: {ex.Message}");
            }
        }
    }
}
