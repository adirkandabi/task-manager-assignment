using TaskManagerAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using TaskManagerAPI.Data;
using TaskManagerAPI.Repositories.Interfaces;

namespace TaskManagerAPI.Repositories.Implementations
{
    /// <summary>
    /// Repository layer for accessing and modifying project data in the database.
    /// </summary>
    /// <remarks>
    /// Uses Entity Framework Core for CRUD operations on projects.
    /// </remarks>
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves all projects including their tasks, with pagination support.
        /// </summary>
        /// <param name="page">The page number (starting from 1).</param>
        /// <param name="pageSize">The number of projects per page.</param>
        /// <returns>A list of all projects.</returns>
        public async Task<List<Project>> GetAllProjectsAsync(int page, int pageSize)
        {
            return await _context.Projects
                .Include(p => p.Tasks)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        /// <summary>
        /// Retrieves all projects for a specific user, with pagination.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of projects per page.</param>
        /// <returns>A list of user-owned projects.</returns>
        public async Task<List<Project>> GetUserProjectsAsync(string userId, int page, int pageSize)
        {
            return await _context.Projects
                .Include(p=>p.Tasks)
                .Where(p => p.UserId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        /// <summary>
        /// Adds a new project to the database.
        /// </summary>
        /// <param name="project">The project to add.</param>
        /// <returns>The added project with its assigned ID.</returns>
        public async Task<Project> AddProjectAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return project;
        }
        /// <summary>
        /// Updates an existing project if the user has access.
        /// </summary>
        /// <param name="updatedProject">The updated project data.</param>
        /// <param name="projectId">The ID of the project to update.</param>
        /// <param name="userId">The user performing the update.</param>
        /// <param name="isAdmin">Indicates if the user is an admin.</param>
        /// <returns>The updated project.</returns>
        public async Task<Project> UpdateProjectAsync(Project updatedProject, int projectId, string userId, bool isAdmin = false)
        {
           
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null) {
                throw new KeyNotFoundException("Project not found.");
            }


            // Check access if not admin
            if (!isAdmin && project.UserId != userId) {
                throw new UnauthorizedAccessException("You are not authorized to update this project.");
            }


            // Update fields
            project.Name = updatedProject.Name;
            project.Description = updatedProject.Description;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            return project;
        }
        /// <summary>
        /// Deletes a project if the user has permission.
        /// </summary>
        /// <param name="projectId">The ID of the project to delete.</param>
        /// <param name="userId">The user attempting to delete.</param>
        /// <param name="isAdmin">Indicates if the user is an admin.</param>
        /// <returns>True if deletion was successful; otherwise false.</returns>
        public async Task<bool> DeleteProjectAsync(int projectId, string userId, bool isAdmin)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null) {
                return false;
            }

            if (!isAdmin && project.UserId != userId) {
                throw new UnauthorizedAccessException("You do not have permission to delete this project.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }


    }

}