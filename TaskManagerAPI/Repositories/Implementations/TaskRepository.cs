using TaskManagerAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using TaskManagerAPI.Data;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Enums;

namespace TaskManagerAPI.Repositories.Implementations
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }
        // Get all tasks for a project with pagination
        public async Task<List<TaskItem>> GetAllProjectTasksAsync(int projectId, int page, int pageSize)
        {
            return await _context.Tasks
                .Where(task=> task.ProjectId == projectId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        // Get tasks for a specific user in a project with pagination
        public async Task<List<TaskItem>> GetUserProjectTasksAsync(int projectId, string userId, int page, int pageSize)
        {
            return await _context.Tasks
               .Where(task =>task.ProjectId == projectId && task.Project.UserId == userId)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
        }
        // Add a new task to a project
        public async Task<TaskItem> AddTaskAsync(TaskItem task,string userId,bool isAdmin=false)
        {
            var project = await _context.Projects.FindAsync(task.ProjectId);
            if (project == null) {
                throw new KeyNotFoundException("project not found.");
            }
            // Check access if not admin
            if(!isAdmin && project.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this task.");
            }
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }
        // Update an existing task in a project
        public async Task<TaskItem> UpdateTaskAsync(TaskItem updatedTask, int taskId,int projectId,string userId, bool isAdmin = false)
        {
           
            var task = await _context.Tasks
                       .Include(t => t.Project) 
                       .FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);

            if (task == null)
            {
                throw new KeyNotFoundException("task not found.");
            }

            // Check access if not admin
            if (!isAdmin && task.Project.UserId != userId)
            {
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
        // Update the status of a task in a project
        public async Task<TaskItem> UpdateTaskStatusAsync(TaskItemStatus status, int taskId, int projectId, string userId, bool isAdmin = false)
        {
            var task = await _context.Tasks
                    .Include(t => t.Project) 
                    .FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);
            if (task == null)
            {
                throw new KeyNotFoundException("task not found.");
            }
            // Check access if not admin
            if (!isAdmin && task.Project.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this task.");

            }
            task.Status = status;
            _context.Tasks.Update(task);
             
            await _context.SaveChangesAsync();

            return task;
        }
        // Delete a task from a project
        public async Task<bool> DeleteTaskAsync(int taskId, int projectId, string userId, bool isAdmin)
        {
            var task = await _context.Tasks
                            .Include(t => t.Project) 
                            .FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);

            if (task == null)
                return false;

            if (!isAdmin && task.Project.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to delete this task.");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
