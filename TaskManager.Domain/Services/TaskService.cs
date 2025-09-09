using TaskManager.Domain.Models;
using TaskStatus = TaskManager.Domain.Models.TaskStatus;

namespace TaskManager.Domain.Services;

public class TaskService : ITaskService
{
    public async Task<TaskItem> CreateAsync(
        string title,
        string? description,
        Func<string, Task<bool>> titleExists)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (await titleExists(title))
            throw new InvalidOperationException("A task with this title already exists.");

        return new TaskItem
        {
            Title = title.Trim(),
            Description = description?.Trim(),
            Status = TaskStatus.Todo
        };
    }
}
