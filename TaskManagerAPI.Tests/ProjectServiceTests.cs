using Xunit;
using Moq;
using FluentAssertions;
using TaskManagerAPI.Services.Implementations;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Models;
using TaskManagerAPI.Dtos;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _mockRepo;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _mockRepo = new Mock<IProjectRepository>();
        _service = new ProjectService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetProjectsAsync_ShouldReturnAllProjects_WhenUserIsAdmin()
    {
        // Arrange
        var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
         {
            new Claim("username", "admin"),
            new Claim("cognito:groups", "admin")
        }));

        var expectedProjects = new List<Project> { new Project { Id = 1 }, new Project { Id = 2 } };
        _mockRepo.Setup(r => r.GetAllProjectsAsync(1, 10)).ReturnsAsync(expectedProjects);

        // Act
        var result = await _service.GetProjectsAsync(adminUser, 1, 10);

        // Assert
        result.Should().BeEquivalentTo(expectedProjects);
    }
    [Fact]
    public async Task GetProjectsAsync_ShouldReturnUserProjects_WhenUserIsNotAdmin()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
        new Claim("username", "user1")
        // No admin group claim
    }));

        var expectedProjects = new List<Project>
    {
        new Project { Id = 1, UserId = "user1" },
        new Project { Id = 2, UserId = "user1" }
    };

        _mockRepo.Setup(r => r.GetUserProjectsAsync("user1", 1, 10)).ReturnsAsync(expectedProjects);

        // Act
        var result = await _service.GetProjectsAsync(user, 1, 10);

        // Assert
        result.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task AddProjectAsync_ShouldReturnCreatedProject_WhenValidDtoAndUser()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("username", "user1")
        }));

        var dto = new ProjectDto { Name = "Test", Description = "TestDesc" };
        var createdProject = new Project { Id = 1, Name = "Test", Description = "TestDesc", UserId = "user1" };

        _mockRepo.Setup(r => r.AddProjectAsync(It.IsAny<Project>())).ReturnsAsync(createdProject);

        // Act
        var result = await _service.AddProjectAsync(dto, user);

        // Assert
        result.Should().BeEquivalentTo(createdProject);
    }
    [Fact]
    public async Task UpdateProjectAsync_ShouldCallRepoAndReturnUpdatedProject()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
        new Claim("username", "user1")
    }));

        var dto = new ProjectDto { Name = "Updated", Description = "UpdatedDesc" };
        var updatedProject = new Project { Id = 1, Name = "Updated", Description = "UpdatedDesc", UserId = "user1" };

        _mockRepo.Setup(r => r.UpdateProjectAsync(It.IsAny<Project>(), 1, "user1", false)).ReturnsAsync(updatedProject);

        // Act
        var result = await _service.UpdateProjectAsync(dto, 1, user);

        // Assert
        result.Should().BeEquivalentTo(updatedProject);
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldReturnTrue_WhenRepoReturnsTrue()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("username", "user1")
        }));

        _mockRepo.Setup(r => r.DeleteProjectAsync(1, "user1", false)).ReturnsAsync(true);

        var result = await _service.DeleteProjectAsync(1, user);

        result.Should().BeTrue();
    }
}
