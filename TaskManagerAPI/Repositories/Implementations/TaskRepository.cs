using TaskManagerAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using TaskManagerAPI.Data;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Enums;

namespace TaskManagerAPI.Repositories.Implementations
{
    /// <summary>
    /// Repository layer for handling task-related database operations.
    /// </summary>
    /// <remarks>
    /// Provides CRUD operations on tasks using Entity Framework Core.
    /// Supports role-based access checks and pagination.
    /// </remarks>
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves all tasks for a specific project with pagination.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="page">The page number (starting from 1).</param>
        /// <param name="pageSize">The number of tasks per page.</param>
        /// <returns>List of tasks in the specified project.</returns>
        public async Task<List<TaskItem>> GetAllProjectTasksAsync(int projectId, int page, int pageSize)
        {
            return await _context.Tasks
                .Where(task=> task.ProjectId == projectId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        /// <summary>
        /// Retrieves tasks for a specific user in a project with pagination.
        /// </summary>
        /// <param name="projectId">The project ID.</param>
        /// <param name="userId">The user's ID.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">Number of tasks per page.</param>
        /// <returns>List of user-specific tasks.</returns>
        public async Task<List<TaskItem>> GetUserProjectTasksAsync(int projectId, string userId, int page, int pageSize)
        {
            return await _context.Tasks
               .Where(task =>task.ProjectId == projectId && task.Project.UserId == userId)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
        }
        /// <summary>
        /// Adds a new task to a project after access verification.
        /// </summary>
        /// <param name="task">The task to add.</param>
        /// <param name="userId">The user attempting to add the task.</param>
        /// <param name="isAdmin">Whether the user is an admin.</param>
        /// <returns>The added task.</returns>
        public async Task<TaskItem> AddTaskAsync(TaskItem task,string userId,bool isAdmin=false)
        {
            var project = await _context.Projects.FindAsync(task.ProjectId);
            if (project == null) {
                throw new KeyNotFoundException("project not found.");
            }
            // Check access if not admin
            if(!isAdmin && project.UserId != userId) {
                throw new UnauthorizedAccessException("You are not authorized to add a task to this project.");
            }
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }
        /// <summary>
        /// Updates a task within a project if the user has access.
        /// </summary>
        /// <param name="updatedTask">The updated task details.</param>
        /// <param name="taskId">The ID of the task to update.</param>
        /// <param name="projectId">The project ID.</param>
        /// <param name="userId">The ID of the user performing the update.</param>
        /// <param name="isAdmin">Whether the user is an admin.</param>
        /// <returns>The updated task.</returns>
        public async Task<TaskItem> UpdateTaskAsync(TaskItem updatedTask, int taskId,int projectId,string userId, bool isAdmin = false)
        {
           
            var task = await _context.Tasks
                       .Include(t => t.Project) 
                       .FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);

            if (task == null) {
                throw new KeyNotFoundException("task not found.");
            }

            // Check access if not admin
            if (!isAdmin && task.Project.UserId != userId){
                throw new UnauthorizedAccessException("You are not authorized to update this task.");

            }

            // Update fields
            task.Name = updatedTask.Name;
            task.Description = updatedTask.Description;
            task.Status = updatedTask.Status;

           _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            return task;
        }
        /// <summary>
        /// Updates only the status of a task within a project.
        /// </summary>
        /// <param name="status">The new status.</param>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="projectId">The project ID.</param>
        /// <param name="userId">The user making the update.</param>
        /// <param name="isAdmin">Whether the user is an admin.</param>
        /// <returns>The updated task.</returns>
        public async Task<TaskItem> UpdateTaskStatusAsync(TaskItemStatus status, int taskId, int projectId, string userId, bool isAdmin = false)
        {
            var task = await _context.Tasks
                    .Include(t => t.Project) 
                    .FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);
            if (task == null) {
                throw new KeyNotFoundException("task not found.");
            }
            // Check access if not admin
            if (!isAdmin && task.Project.UserId != userId) {
                throw new UnauthorizedAccessException("You are not authorized to update this task.");

            }
            task.Status = status;
            _context.Tasks.Update(task);
            
            await _context.SaveChangesAsync();

            return task;
        }
        /// <summary>
        /// Deletes a task from a project if the user is authorized.
        /// </summary>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <param name="projectId">The project ID.</param>
        /// <param name="userId">The user's ID.</param>
        /// <param name="isAdmin">Whether the user is an admin.</param>
        /// <returns>True if the task was deleted; otherwise false.</returns>
        public async Task<bool> DeleteTaskAsync(int taskId, int projectId, string userId, bool isAdmin) {
            var task = await _context.Tasks
                            .Include(t => t.Project)
                            .FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);

            if (task == null) {
                return false;
            }
            // Check access if not admin
            if (!isAdmin && task.Project.UserId != userId) {
                throw new UnauthorizedAccessException("You do not have permission to delete this task.");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
