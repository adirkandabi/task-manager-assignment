using TaskManagerAPI.Models;
using System.Security.Claims;
using TaskManagerAPI.Dtos;

namespace TaskManagerAPI.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<Project>> GetProjectsAsync(ClaimsPrincipal user, int page, int pageSize);
        Task<Project> AddProjectAsync(ProjectDto project, ClaimsPrincipal user);
        Task<Project> UpdateProjectAsync(ProjectDto updatedProject, int projectId, ClaimsPrincipal user);
        Task<bool> DeleteProjectAsync(int projectId, ClaimsPrincipal user);

    }
}
