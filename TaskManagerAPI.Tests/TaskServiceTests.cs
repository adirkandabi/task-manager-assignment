using Xunit;
using Moq;
using FluentAssertions;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerAPI.Models;
using TaskManagerAPI.Enums;
using TaskManagerAPI.Dtos;
using TaskManagerAPI.Services.Implementations;
using TaskManagerAPI.Repositories.Interfaces;
using TaskManagerAPI.Helpers;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockRepo;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _mockRepo = new Mock<ITaskRepository>();
        _service = new TaskService(_mockRepo.Object);
    }

    private ClaimsPrincipal CreateUser(string username, bool isAdmin = false)
    {
        var claims = new List<Claim> { new Claim("username", username) };
        if (isAdmin)
            claims.Add(new Claim("cognito:groups", "admin"));

        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }

    [Fact]
    public async Task GetTasksAsync_ShouldReturnAdminTasks_WhenUserIsAdmin()
    {
        var user = CreateUser("admin", true);
        var expected = new List<TaskItem> { new TaskItem { Id = 1 }, new TaskItem { Id = 2 } };
        _mockRepo.Setup(r => r.GetAllProjectTasksAsync(1, 1, 10)).ReturnsAsync(expected);

        var result = await _service.GetTasksAsync(user, 1, 1, 10);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetTasksAsync_ShouldReturnUserTasks_WhenUserIsNotAdmin()
    {
        var user = CreateUser("user1");
        var expected = new List<TaskItem> { new TaskItem { Id = 1 }, new TaskItem { Id = 2 } };
        _mockRepo.Setup(r => r.GetUserProjectTasksAsync(1, "user1", 1, 10)).ReturnsAsync(expected);

        var result = await _service.GetTasksAsync(user, 1, 1, 10);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task AddTaskAsync_ShouldReturnCreatedTask()
    {
        var user = CreateUser("user1");
        var dto = new TaskDto { Name = "Task1", Description = "Desc", Status = TaskItemStatus.Todo };
        var expected = new TaskItem { Id = 1, Name = "Task1", Description = "Desc", Status = TaskItemStatus.Todo, ProjectId = 1 };

        _mockRepo.Setup(r => r.AddTaskAsync(It.IsAny<TaskItem>(), "user1", false)).ReturnsAsync(expected);

        var result = await _service.AddTaskAsync(dto, 1, user);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldReturnUpdatedTask()
    {
        var user = CreateUser("user1");
        var dto = new TaskDto { Name = "Updated", Description = "UpdatedDesc", Status = TaskItemStatus.InProgress };
        var expected = new TaskItem { Id = 1, Name = "Updated", Description = "UpdatedDesc", Status = TaskItemStatus.InProgress, ProjectId = 1 };

        _mockRepo.Setup(r => r.UpdateTaskAsync(It.IsAny<TaskItem>(), 1, 1, "user1", false)).ReturnsAsync(expected);

        var result = await _service.UpdateTaskAsync(dto, 1, 1, user);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_ShouldReturnUpdatedStatus()
    {
        var user = CreateUser("user1");
        var expected = new TaskItem { Id = 1, Name = "Task1", Status = TaskItemStatus.Done };

        _mockRepo.Setup(r => r.UpdateTaskStatusAsync(TaskItemStatus.Done, 1, 1, "user1", false)).ReturnsAsync(expected);

        var result = await _service.UpdateTaskStatusAsync(TaskItemStatus.Done, 1, 1, user);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnTrue_WhenDeleted()
    {
        var user = CreateUser("user1");

        _mockRepo.Setup(r => r.DeleteTaskAsync(1, 1, "user1", false)).ReturnsAsync(true);

        var result = await _service.DeleteTaskAsync(1, 1, user);

        result.Should().BeTrue();
    }
}
