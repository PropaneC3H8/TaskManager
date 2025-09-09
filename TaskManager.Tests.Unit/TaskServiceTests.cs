using FluentAssertions;
using TaskManager.Domain.Services;

public class TaskServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Task_When_Valid()
    {
        var service = new TaskService();
        async Task<bool> exists(string _) => false;

        var task = await service.CreateAsync(" Write tests ", " first pass ", exists);

        task.Title.Should().Be("Write tests");
        task.Description.Should().Be("first pass");
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Title_Missing()
    {
        var service = new TaskService();
        async Task<bool> exists(string _) => false;

        var act = async () => await service.CreateAsync("", null, exists);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Title_Exists()
    {
        var service = new TaskService();
        async Task<bool> exists(string t) => t.Trim().Equals("Duplicate", StringComparison.OrdinalIgnoreCase);

        var act = async () => await service.CreateAsync("Duplicate", "desc", exists);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
