using TaskManagerAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using TaskManagerAPI.Enums;

namespace TaskManagerAPI.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetAllProjectTasksAsync(int projectId,int page, int pageSize);
        Task<List<TaskItem>> GetUserProjectTasksAsync(int projectId,string userId, int page, int pageSize);
        Task<TaskItem> AddTaskAsync(TaskItem task,string userId,bool isAdmin=false);
        Task<TaskItem> UpdateTaskAsync(TaskItem updatedTask,int taskId,int projectId,string userId,bool isAdmin=false);
        Task<TaskItem> UpdateTaskStatusAsync(TaskItemStatus status,int taskId, int projectId, string userId, bool isAdmin = false);
        Task<bool> DeleteTaskAsync(int taskId, int projectId, string userId, bool isAdmin=false);
    }
}
