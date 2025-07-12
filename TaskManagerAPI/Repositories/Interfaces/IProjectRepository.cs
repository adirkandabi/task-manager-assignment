using TaskManagerAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace TaskManagerAPI.Repositories.Interfaces
{
	public interface IProjectRepository
	{
		Task<List<Project>> GetAllProjectsAsync(int page, int pageSize);
		Task<List<Project>> GetUserProjectsAsync(string userId, int page, int pageSize);
        Task<Project> AddProjectAsync(Project project);
		Task<Project> UpdateProjectAsync(Project updatedProject, int projectId, string userId, bool isAdmin = false);
		Task<bool> DeleteProjectAsync(int projectId, string userId, bool isAdmin);
    }
}
