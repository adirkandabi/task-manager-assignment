using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using TaskManagerAPI.Controllers;
using TaskManagerAPI.Services.Interfaces;
using TaskManagerAPI.Models;
using Microsoft.AspNetCore.Http;
using TaskManagerAPI.Dtos;

public class ProjectsControllerTests
{
	private readonly Mock<IProjectService> _projectServiceMock;
	private readonly Mock<ILogger<ProjectsController>> _loggerMock;
	private readonly ProjectsController _controller;

	public ProjectsControllerTests()
	{
		_projectServiceMock = new Mock<IProjectService>();
		_loggerMock = new Mock<ILogger<ProjectsController>>();
		_controller = new ProjectsController(_projectServiceMock.Object, _loggerMock.Object);
		_controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext
			{
				User = new ClaimsPrincipal(new ClaimsIdentity(new[] {
					new Claim(ClaimTypes.NameIdentifier, "user-id")
				}, "mock"))
			}
		};
	}

	[Fact]
	public async Task GetProjects_ReturnsOkResult_WithProjects()
	{
		// Arrange
		_projectServiceMock.Setup(s => s.GetProjectsAsync(It.IsAny<ClaimsPrincipal>(), 1, 10))
			.ReturnsAsync(new List<Project> { new Project { Id = 1, Name = "Test" } });

		// Act
		var result = await _controller.GetProjects(1, 10);

		// Assert
		result.Should().BeOfType<OkObjectResult>();
	}

	[Fact]
	public async Task CreateProject_ReturnsCreatedResult()
	{
		var dto = new ProjectDto { Name = "New Project" };
		var created = new Project { Id = 123, Name = "New Project" };

		_projectServiceMock.Setup(s => s.AddProjectAsync(dto, It.IsAny<ClaimsPrincipal>()))
			.ReturnsAsync(created);

		var result = await _controller.CreateProject(dto);

		result.Should().BeOfType<CreatedAtActionResult>();
		((CreatedAtActionResult)result).Value.Should().BeEquivalentTo(created);
	}

	[Fact]
	public async Task UpdateProject_ReturnsOk_WhenSuccess()
	{
		var dto = new ProjectDto { Name = "Updated" };
		var updated = new Project { Id = 1, Name = "Updated" };

		_projectServiceMock.Setup(s => s.UpdateProjectAsync(dto, 1, It.IsAny<ClaimsPrincipal>()))
			.ReturnsAsync(updated);

		var result = await _controller.UpdateProject(1, dto);

		result.Should().BeOfType<OkObjectResult>();
	}

	[Fact]
	public async Task UpdateProject_Returns403_IfUnauthorized()
	{
		var dto = new ProjectDto();
		_projectServiceMock.Setup(s => s.UpdateProjectAsync(dto, 1, It.IsAny<ClaimsPrincipal>()))
			.ThrowsAsync(new UnauthorizedAccessException("Not allowed"));

		var result = await _controller.UpdateProject(1, dto);

		result.Should().BeOfType<ObjectResult>()
			  .Which.StatusCode.Should().Be(403);
	}

	[Fact]
	public async Task UpdateProject_Returns404_IfNotFound()
	{
		var dto = new ProjectDto();
		_projectServiceMock.Setup(s => s.UpdateProjectAsync(dto, 1, It.IsAny<ClaimsPrincipal>()))
			.ThrowsAsync(new KeyNotFoundException("Not found"));

		var result = await _controller.UpdateProject(1, dto);

		result.Should().BeOfType<NotFoundObjectResult>();
	}

	[Fact]
	public async Task DeleteProject_ReturnsNoContent_WhenDeleted()
	{
		_projectServiceMock.Setup(s => s.DeleteProjectAsync(1, It.IsAny<ClaimsPrincipal>()))
			.ReturnsAsync(true);

		var result = await _controller.DeleteProject(1);

		result.Should().BeOfType<NoContentResult>();
	}

	[Fact]
	public async Task DeleteProject_ReturnsNotFound_WhenMissing()
	{
		_projectServiceMock.Setup(s => s.DeleteProjectAsync(1, It.IsAny<ClaimsPrincipal>()))
			.ReturnsAsync(false);

		var result = await _controller.DeleteProject(1);

		result.Should().BeOfType<NotFoundObjectResult>();
	}

	[Fact]
	public async Task DeleteProject_Returns403_IfUnauthorized()
	{
		_projectServiceMock.Setup(s => s.DeleteProjectAsync(1, It.IsAny<ClaimsPrincipal>()))
			.ThrowsAsync(new UnauthorizedAccessException("Not allowed"));

		var result = await _controller.DeleteProject(1);

		result.Should().BeOfType<ObjectResult>()
			  .Which.StatusCode.Should().Be(403);
	}
}
