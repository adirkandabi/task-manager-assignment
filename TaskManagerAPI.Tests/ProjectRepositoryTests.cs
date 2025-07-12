
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories.Implementations;
using System.Threading.Tasks;
using System.Collections.Generic;
using TaskManagerAPI.Data;

public class ProjectRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly ProjectRepository _repository;

    public ProjectRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ProjectRepository(_context);
    }

    [Fact]
    public async Task AddProjectAsync_ShouldAddProjectToDatabase()
    {
        var project = new Project { Name = "Test", Description = "Demo", UserId = "user1" };

        var result = await _repository.AddProjectAsync(project);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        (await _context.Projects.FindAsync(result.Id)).Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllProjectsAsync_ShouldReturnAllProjects()
    {
        _context.Projects.AddRange(
            new Project { Name = "One", Description = "", UserId = "u1" },
            new Project { Name = "Two", Description = "", UserId = "u2" }
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllProjectsAsync(1, 10);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetUserProjectsAsync_ShouldReturnOnlyUserProjects()
    {
        _context.Projects.AddRange(
            new Project { Name = "User1 Project", Description = "", UserId = "user1" },
            new Project { Name = "Other User Project", Description = "", UserId = "user2" }
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetUserProjectsAsync("user1", 1, 10);

        result.Should().OnlyContain(p => p.UserId == "user1");
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldUpdateIfAuthorized()
    {
        var original = new Project { Name = "Old", Description = "Old", UserId = "user1" };
        _context.Projects.Add(original);
        await _context.SaveChangesAsync();

        var updated = new Project { Name = "New", Description = "New", UserId = "user1" };
        var result = await _repository.UpdateProjectAsync(updated, original.Id, "user1", false);

        result.Name.Should().Be("New");
        result.Description.Should().Be("New");
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldDeleteIfAuthorized()
    {
        var project = new Project { Name = "ToDelete", Description = "", UserId = "user1" };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteProjectAsync(project.Id, "user1", false);

        result.Should().BeTrue();
        (await _context.Projects.FindAsync(project.Id)).Should().BeNull();
    }
}
