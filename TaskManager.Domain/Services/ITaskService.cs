using TaskManager.Domain.Models;

namespace TaskManager.Domain.Services;

public interface ITaskService
{
    Task<TaskItem> CreateAsync(string title, string? description, Func<string, Task<bool>> titleExists);
}