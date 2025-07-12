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
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public async Task<List<TaskItem>> GetTasksAsync(ClaimsPrincipal user, int projectId, int page, int pageSize)
        {
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID is missing.");
            }
            bool isAdmin = Helpers.Helpers.IsAdmin(user);
            return isAdmin
                ? await _taskRepository.GetAllProjectTasksAsync(projectId, page, pageSize)
                : await _taskRepository.GetUserProjectTasksAsync(projectId, userId, page, pageSize);
        }
        public async Task<TaskItem> AddTaskAsync(TaskDto task, int projectId, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID is missing.");
            }

            var newTask = new TaskItem
            {
                Name = task.Name,
                Description = task.Description,
                Status = task.Status,
                ProjectId = projectId,
            };
            bool isAdmin = Helpers.Helpers.IsAdmin(user);
            return await _taskRepository.AddTaskAsync(newTask, userId, isAdmin);
        }
        public async Task<TaskItem> UpdateTaskAsync(TaskDto updatedTaskDto, int taskId,int projectId, ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue("username");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User ID is missing.");
                }
                bool isAdmin = Helpers.Helpers.IsAdmin(user);
                var updatedTask = new TaskItem
                {
                    Name = updatedTaskDto.Name,
                    Description = updatedTaskDto.Description,
                    Status = updatedTaskDto.Status,
                    ProjectId = projectId,
                };
                return await _taskRepository.UpdateTaskAsync(updatedTask, taskId,projectId, userId, isAdmin);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        public async Task<TaskItem> UpdateTaskStatusAsync(TaskItemStatus status, int taskId, int projectId, ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue("username");
                if(string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User ID is missing.");
                }
                bool isAdmin = Helpers.Helpers.IsAdmin(user);
                return await _taskRepository.UpdateTaskStatusAsync(status, taskId,projectId, userId, isAdmin);
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public async Task<bool> DeleteTaskAsync(int taskId, int projectId, ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue("username");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User ID is missing.");
                }
                bool isAdmin = Helpers.Helpers.IsAdmin(user);
                return await _taskRepository.DeleteTaskAsync(taskId,projectId, userId, isAdmin);
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }
        
    }
}
