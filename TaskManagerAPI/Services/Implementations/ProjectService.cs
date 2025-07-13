using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services.Interfaces;
using System.Security.Claims;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Dtos;

namespace TaskManagerAPI.Services.Implementations
{
    /// <summary>
    /// Service layer for managing project-related business logic.
    /// </summary>
    /// <remarks>
    /// Coordinates between controllers and the project repository, and handles user permissions.
    /// </remarks>
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        /// <summary>
        /// Retrieves a list of projects for the current user, or all projects if the user is an admin.
        /// </summary>
        /// <param name="user">The authenticated user (ClaimsPrincipal).</param>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>List of projects.</returns>
        public async Task<List<Project>> GetProjectsAsync(ClaimsPrincipal user, int page, int pageSize)
        {
            // If user is authenticated, retreive their ID
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId)) {
                throw new UnauthorizedAccessException("User ID is missing.");
            }

            bool isAdmin = Helpers.Helpers.IsAdmin(user);


            return isAdmin
                ? await _projectRepository.GetAllProjectsAsync(page, pageSize)
                : await _projectRepository.GetUserProjectsAsync(userId, page, pageSize);
        }
        /// <summary>
        /// Creates a new project and assigns it to the current user.
        /// </summary>
        /// <param name="project">The project DTO containing name and description.</param>
        /// <param name="user">The authenticated user creating the project.</param>
        /// <returns>The newly created project.</returns>
        public async Task<Project> AddProjectAsync(ProjectDto project, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId)) {
                throw new UnauthorizedAccessException("User ID is missing.");
            }
           // Map ProjectDto to Project entity
            var newProject = new Project {
                Name = project.Name,
                Description = project.Description,
                UserId = userId,
            };
            return await _projectRepository.AddProjectAsync(newProject);
        }
        /// <summary>
        /// Updates a project if the user is authorized.
        /// </summary>
        /// <param name="updatedProjectDto">DTO with updated name and description.</param>
        /// <param name="projectId">The ID of the project to update.</param>
        /// <param name="user">The authenticated user performing the update.</param>
        /// <returns>The updated project entity.</returns>
        public async Task<Project> UpdateProjectAsync(ProjectDto updatedProjectDto, int projectId, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId)) {
                throw new UnauthorizedAccessException("User ID is missing.");
            }
            bool isAdmin = Helpers.Helpers.IsAdmin(user);
            // Map ProjectDto to Project entity
            var updatedProject = new Project {
                Name = updatedProjectDto.Name,
                Description = updatedProjectDto.Description,
                UserId = userId,
            };
            return await _projectRepository.UpdateProjectAsync(updatedProject, projectId, userId, isAdmin);
        }
        /// <summary>
        /// Deletes a project if the user is the owner or an admin.
        /// </summary>
        /// <param name="projectId">The ID of the project to delete.</param>
        /// <param name="user">The authenticated user requesting the deletion.</param>
        /// <returns>True if the project was deleted successfully, otherwise false.</returns>
        public async Task<bool> DeleteProjectAsync(int projectId, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId)) {
                throw new UnauthorizedAccessException("User ID is missing.");
            }
            bool isAdmin = Helpers.Helpers.IsAdmin(user);
            return await _projectRepository.DeleteProjectAsync(projectId, userId, isAdmin);
        }
    }
}
