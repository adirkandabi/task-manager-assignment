using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Services.Interfaces;
using System.Security.Claims;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Dtos;

namespace TaskManagerAPI.Services.Implementations
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<List<Project>> GetProjectsAsync(ClaimsPrincipal user, int page, int pageSize)
        {
            var userId = user.FindFirstValue("username");
          
            bool isAdmin = Helpers.Helpers.IsAdmin(user);


            return isAdmin
                ? await _projectRepository.GetAllProjectsAsync(page, pageSize)
                : await _projectRepository.GetUserProjectsAsync(userId, page, pageSize);
        }
        public async Task<Project> AddProjectAsync(ProjectDto project, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("username");
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID is missing.");
            }
           
            var newProject = new Project
            {
                Name = project.Name,
                Description = project.Description,
                UserId = userId,
            };
            return await _projectRepository.AddProjectAsync(newProject);
        }
        public async Task<Project> UpdateProjectAsync(ProjectDto updatedProjectDto, int projectId, ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue("username");
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User ID is missing.");

               
                bool isAdmin =Helpers.Helpers.IsAdmin(user);
                var updatedProject = new Project
                {
                    Name = updatedProjectDto.Name,
                    Description = updatedProjectDto.Description,
                    UserId = userId,
                };
                return await _projectRepository.UpdateProjectAsync(updatedProject, projectId, userId, isAdmin);
            }
            catch (Exception ex)
            {
                // Log exception (use logger in real app)
                Console.WriteLine($"Error updating project: {ex.Message}");
                throw; // Let controller handle final response
            }
        }
        public async Task<bool> DeleteProjectAsync(int projectId, ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue("username");
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User ID is missing.");

               
                bool isAdmin = Helpers.Helpers.IsAdmin(user);

                return await _projectRepository.DeleteProjectAsync(projectId, userId, isAdmin);
            }
            catch
            {
                throw; 
            }
        }
    }
}
