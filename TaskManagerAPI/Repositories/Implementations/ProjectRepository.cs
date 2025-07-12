using TaskManagerAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using TaskManagerAPI.Data;
using TaskManagerAPI.Repositories.Interfaces;

namespace TaskManagerAPI.Repositories.Implementations
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetAllProjectsAsync(int page, int pageSize)
        {
            return await _context.Projects
                .Include(p => p.Tasks)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId, int page, int pageSize)
        {
            return await _context.Projects
                .Include(p=>p.Tasks)
                .Where(p => p.UserId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<Project> AddProjectAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return project;
        }
        public async Task<Project> UpdateProjectAsync(Project updatedProject, int projectId, string userId, bool isAdmin = false)
        {
           
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
                throw new KeyNotFoundException("Project not found.");

            // Check access if not admin
            if (!isAdmin && project.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to update this project.");

            // Update fields
            project.Name = updatedProject.Name;
            project.Description = updatedProject.Description;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            return project;
        }
        public async Task<bool> DeleteProjectAsync(int projectId, string userId, bool isAdmin)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
                return false;

            if (!isAdmin && project.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to delete this project.");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }


    }

}