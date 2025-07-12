using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.Services.Interfaces;
using System.Threading.Tasks;
using TaskManagerAPI.Models;
using TaskManagerAPI.Dtos;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProjects([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var projects = await _projectService.GetProjectsAsync(User, page, pageSize);
                return Ok(projects);
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto project)
        {
            try
            {
                var createdProject = await _projectService.AddProjectAsync(project, User);
                return CreatedAtAction(nameof(GetProjects), new { id = createdProject.Id }, createdProject);
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpPut("{projectId}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject(int projectId, [FromBody] ProjectDto project)
        {
            try
            {
                var updated = await _projectService.UpdateProjectAsync(project, projectId, User);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(403, ex.Message);
            }

            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                // Generic fallback
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpDelete("{projectId}")]
        [Authorize]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            try
            {
                var success = await _projectService.DeleteProjectAsync(projectId, User);
                if (!success)
                {
                    return NotFound("Project not found");
                }

                return NoContent(); 
            }
            catch (UnauthorizedAccessException ex) {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "An error occurred while deleting the project.");
            }
        }




    }
}
