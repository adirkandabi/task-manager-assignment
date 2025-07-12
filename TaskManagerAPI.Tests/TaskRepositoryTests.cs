using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;
using TaskManagerAPI.Enums;
using TaskManagerAPI.Data;
using TaskManagerAPI.Repositories.Implementations;
using System.Threading.Tasks;
using System.Collections.Generic;

public class TaskRepositoryTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddTaskAsync_ShouldAddTaskSuccessfully()
    {
        using var context = CreateContext();
        var repository = new TaskRepository(context);

        var project = new Project { Name = "Project 1", Description = "Test", UserId = "user1" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var task = new TaskItem
        {
            Name = "Task",
            Description = "Test task",
            Status = TaskItemStatus.Todo,
            ProjectId = project.Id
        };

        var result = await repository.AddTaskAsync(task, "user1");

        result.Should().NotBeNull();
        result.Name.Should().Be("Task");
    }

    [Fact]
    public async Task GetAllProjectTasksAsync_ShouldReturnTasks()
    {
        using var context = CreateContext();
        var repository = new TaskRepository(context);

        var project = new Project { Name = "Project", Description = "", UserId = "user1" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        context.Tasks.AddRange(
            new TaskItem { Name = "Task 1", Status = TaskItemStatus.Todo, ProjectId = project.Id },
            new TaskItem { Name = "Task 2", Status = TaskItemStatus.Done, ProjectId = project.Id }
        );
        await context.SaveChangesAsync();

        var result = await repository.GetAllProjectTasksAsync(project.Id, 1, 10);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateTask()
    {
        using var context = CreateContext();
        var repository = new TaskRepository(context);

        var project = new Project { Name = "Project", Description = "", UserId = "user1" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var task = new TaskItem
        {
            Name = "Old Name",
            Description = "Old Desc",
            Status = TaskItemStatus.Todo,
            ProjectId = project.Id
        };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var updated = new TaskItem
        {
            Name = "New Name",
            Description = "New Desc",
            Status = TaskItemStatus.InProgress,
            ProjectId = project.Id
        };

        var result = await repository.UpdateTaskAsync(updated, task.Id, project.Id, "user1");

        result.Name.Should().Be("New Name");
        result.Description.Should().Be("New Desc");
        result.Status.Should().Be(TaskItemStatus.InProgress);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_ShouldUpdateStatus()
    {
        using var context = CreateContext();
        var repository = new TaskRepository(context);

        var project = new Project { Name = "Project", Description = "", UserId = "user1" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var task = new TaskItem { Name = "Task", Status = TaskItemStatus.Todo, ProjectId = project.Id };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var result = await repository.UpdateTaskStatusAsync(TaskItemStatus.Done, task.Id, project.Id, "user1");

        result.Status.Should().Be(TaskItemStatus.Done);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnTrue_WhenDeleted()
    {
        using var context = CreateContext();
        var repository = new TaskRepository(context);

        var project = new Project { Name = "Project", Description = "", UserId = "user1" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var task = new TaskItem { Name = "To Delete", Status = TaskItemStatus.Todo, ProjectId = project.Id };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var result = await repository.DeleteTaskAsync(task.Id, project.Id, "user1",false);

        result.Should().BeTrue();
    }
}
