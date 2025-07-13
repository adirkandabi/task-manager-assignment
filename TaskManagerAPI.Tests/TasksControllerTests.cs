using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagerAPI.Controllers;
using TaskManagerAPI.Dtos;
using TaskManagerAPI.Models;
using TaskManagerAPI.Enums;
using TaskManagerAPI.Services.Interfaces;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly Mock<ILogger<TasksController>> _loggerMock;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _taskServiceMock = new Mock<ITaskService>();
        _loggerMock = new Mock<ILogger<TasksController>>();

        _controller = new TasksController(_taskServiceMock.Object, _loggerMock.Object);
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
    public async Task GetTasks_ReturnsOk()
    {
        _taskServiceMock.Setup(x => x.GetTasksAsync(It.IsAny<ClaimsPrincipal>(), 1, 1, 10))
            .ReturnsAsync(new List<TaskItem>());

        var result = await _controller.GetTasks(1, 1, 10);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AddTask_ReturnsOk_WhenAuthorized()
    {
        var dto = new TaskDto { Name = "Task A" };
        var created = new TaskItem { Id = 1, Name = "Task A" };

        _taskServiceMock.Setup(x => x.AddTaskAsync(dto, 1, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(created);

        var result = await _controller.AddTask(1, dto);

        result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result).Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task AddTask_ReturnsUnauthorized_OnAccessException()
    {
        var dto = new TaskDto { Name = "Unauthorized Task" };

        _taskServiceMock.Setup(x => x.AddTaskAsync(dto, 1, It.IsAny<ClaimsPrincipal>()))
            .ThrowsAsync(new UnauthorizedAccessException("No access"));

        var result = await _controller.AddTask(1, dto);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task UpdateTask_ReturnsOk_WhenSuccessful()
    {
        var dto = new TaskDto { Name = "Updated Task" };
        var updated = new TaskItem { Id = 1, Name = "Updated Task" };

        _taskServiceMock.Setup(x => x.UpdateTaskAsync(dto, 1, 1, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(updated);

        var result = await _controller.UpdateTask(1, 1, dto);

        result.Should().BeOfType<OkObjectResult>()
              .Which.Value.Should().BeEquivalentTo(updated);
    }

    [Fact]
    public async Task UpdateTask_ReturnsNotFound_WhenMissing()
    {
        var dto = new TaskDto();

        _taskServiceMock.Setup(x => x.UpdateTaskAsync(dto, 1, 1, It.IsAny<ClaimsPrincipal>()))
            .ThrowsAsync(new KeyNotFoundException("Not found"));

        var result = await _controller.UpdateTask(1, 1, dto);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateTaskStatus_ReturnsOk_WhenSuccessful()
    {
        var updated = new TaskItem { Id = 1, Status = TaskItemStatus.Done };

        _taskServiceMock.Setup(x => x.UpdateTaskStatusAsync(TaskItemStatus.Done, 1, 1, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(updated);

        var result = await _controller.UpdateTaskStatus(1, 1, TaskItemStatus.Done);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UpdateTaskStatus_Returns403_IfUnauthorized()
    {
        _taskServiceMock.Setup(x => x.UpdateTaskStatusAsync(TaskItemStatus.InProgress, 1, 1, It.IsAny<ClaimsPrincipal>()))
            .ThrowsAsync(new UnauthorizedAccessException("No permission"));

        var result = await _controller.UpdateTaskStatus(1, 1, TaskItemStatus.InProgress);

        result.Should().BeOfType<ObjectResult>()
              .Which.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task DeleteTask_ReturnsOk_WhenDeleted()
    {
        _taskServiceMock.Setup(x => x.DeleteTaskAsync(1, 1, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(true);

        var result = await _controller.DeleteTask(1, 1);

        result.Should().BeOfType<OkObjectResult>()
              .Which.Value.Should().Be("Task deleted successfully");
    }

    [Fact]
    public async Task DeleteTask_ReturnsNotFound_IfNotExist()
    {
        _taskServiceMock.Setup(x => x.DeleteTaskAsync(1, 1, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(false);

        var result = await _controller.DeleteTask(1, 1);

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
