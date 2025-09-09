using System.Net.Http.Json;
using FluentAssertions;
using TaskManager.Domain.Models;

public class TasksApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TasksApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_Then_GetById_Should_Return_Created_Task()
    {
        var dto = new { title = "Integration", description = "End-to-end check" };
        var post = await _client.PostAsJsonAsync("/api/tasks", dto);
        post.EnsureSuccessStatusCode();

        var created = await post.Content.ReadFromJsonAsync<TaskItem>();
        created.Should().NotBeNull();
        created!.Id.Should().BeGreaterThan(0);
        created.Title.Should().Be("Integration");

        var get = await _client.GetAsync($"/api/tasks/{created.Id}");
        get.EnsureSuccessStatusCode();
        var fetched = await get.Content.ReadFromJsonAsync<TaskItem>();
        fetched!.Id.Should().Be(created.Id);
        fetched.Title.Should().Be("Integration");
    }

    [Fact]
    public async Task Get_All_Should_Return_List()
    {
        var res = await _client.GetAsync("/api/tasks");
        res.EnsureSuccessStatusCode();
        var items = await res.Content.ReadFromJsonAsync<List<TaskItem>>();
        items.Should().NotBeNull();
    }
}