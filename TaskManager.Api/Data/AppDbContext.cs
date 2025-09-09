using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Models;

namespace TaskManager.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}
