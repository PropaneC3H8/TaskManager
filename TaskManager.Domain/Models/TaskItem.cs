namespace TaskManager.Domain.Models;

public enum TaskStatus { Todo = 0, InProgress = 1, Done = 2 }

public class TaskItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}