using TaskManagerAPI.Models;
using System.Security.Claims;
using TaskManagerAPI.Enums;
using TaskManagerAPI.Dtos;

namespace TaskManagerAPI.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetTasksAsync(ClaimsPrincipal user,int projectId, int page, int pageSize);
        Task<TaskItem> AddTaskAsync(TaskDto task,int projectId, ClaimsPrincipal user);
        Task<TaskItem> UpdateTaskAsync(TaskDto updatedTask,int taskId,int projectId, ClaimsPrincipal user);
        Task<TaskItem> UpdateTaskStatusAsync(TaskItemStatus status,int taskId,int projectId,ClaimsPrincipal user);
        Task<bool> DeleteTaskAsync(int taskId, int projectId, ClaimsPrincipal user);

    }
}
