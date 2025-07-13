using TaskManagerAPI.Models;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services.Interfaces;
using System.Security.Claims;
using TaskManagerAPI.Repositories.Implementations;
using TaskManagerAPI.Enums;
using TaskManagerAPI.Dtos;

namespace TaskManagerAPI.Services.Implementations
{
    /// <summary>
    /// Service layer for managing tasks, including logic for authentication and authorization.
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        /// <summary>
        /// Retrieves tasks for a given project, filtered by user or returns all if admin.
        /// </summary>
        /// <param name="user">The logged-in user's claims.</param>
        /// <param name="projectId">The project ID.</param>
        /// <param name="page">Pagination page number.</param>
        /// <param name="pageSize">Pagination page size.</param>
        /// <returns>List of TaskItem objects.</returns>
        public async Task<List<TaskItem>> GetTasksAsync(ClaimsPrincipal user, int projectId, int page, int pageSize)
        {
            // If user is authenticated, retrieve their ID
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId)) {
                throw new UnauthorizedAccessException("User ID is missing.");
            }
            bool isAdmin = Helpers.Helpers.IsAdmin(user);
            return isAdmin
                ? await _taskRepository.GetAllProjectTasksAsync(projectId, page, pageSize)
                : await _taskRepository.GetUserProjectTasksAsync(projectId, userId, page, pageSize);
        }
        /// <summary>
        /// Adds a new task to the specified project.
        /// </summary>
        /// <param name="task">Task DTO with task details.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="user">The logged-in user's claims.</param>
        /// <returns>The created TaskItem.</returns>
        public async Task<TaskItem> AddTaskAsync(TaskDto task, int projectId, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId)) {
                throw new UnauthorizedAccessException("User ID is missing.");
            }
            // Map TaskDto to TaskItem entity
            var newTask = new TaskItem {
                Name = task.Name,
                Description = task.Description,
                Status = task.Status,
                ProjectId = projectId,
            };
            bool isAdmin = Helpers.Helpers.IsAdmin(user);
            return await _taskRepository.AddTaskAsync(newTask, userId, isAdmin);
        }
        /// <summary>
        /// Updates an existing task's full details.
        /// </summary>
        /// <param name="updatedTaskDto">Updated task details.</param>
        /// <param name="taskId">The ID of the task to update.</param>
        /// <param name="projectId">The project to which the task belongs.</param>
        /// <param name="user">The logged-in user's claims.</param>
        /// <returns>The updated TaskItem.</returns>
        public async Task<TaskItem> UpdateTaskAsync(TaskDto updatedTaskDto, int taskId,int projectId, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId)) {
                throw new UnauthorizedAccessException("User ID is missing.");
            }
            bool isAdmin = Helpers.Helpers.IsAdmin(user);
            var updatedTask = new TaskItem {
                Name = updatedTaskDto.Name,
                Description = updatedTaskDto.Description,
                Status = updatedTaskDto.Status,
                ProjectId = projectId,
            };
            return await _taskRepository.UpdateTaskAsync(updatedTask, taskId, projectId, userId, isAdmin);
        }
        /// <summary>
        /// Updates the status of a task (e.g., to done, in-progress).
        /// </summary>
        /// <param name="status">New task status.</param>
        /// <param name="taskId">ID of the task.</param>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="user">The logged-in user's claims.</param>
        /// <returns>The updated TaskItem with new status.</returns>
        public async Task<TaskItem> UpdateTaskStatusAsync(TaskItemStatus status, int taskId, int projectId, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId)) {
                throw new UnauthorizedAccessException("User ID is missing.");
            }
            bool isAdmin = Helpers.Helpers.IsAdmin(user);
            return await _taskRepository.UpdateTaskStatusAsync(status, taskId, projectId, userId, isAdmin);
        }
        /// <summary>
        /// Deletes a task from the specified project.
        /// </summary>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <param name="projectId">The project to which the task belongs.</param>
        /// <param name="user">The logged-in user's claims.</param>
        /// <returns>True if deletion succeeded, otherwise false.</returns>
        public async Task<bool> DeleteTaskAsync(int taskId, int projectId, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId)) {
                throw new UnauthorizedAccessException("User ID is missing.");
            }
            bool isAdmin = Helpers.Helpers.IsAdmin(user);
            return await _taskRepository.DeleteTaskAsync(taskId, projectId, userId, isAdmin);

        }
        
    }
}
