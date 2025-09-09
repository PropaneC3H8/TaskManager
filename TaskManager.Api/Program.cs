using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

// Use SQLite file locally; integration tests will override to in-memory SQLite
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=tasks.db;Cache=Shared;Pooling=True"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");     // better read/write concurrency
    db.Database.ExecuteSqlRaw("PRAGMA busy_timeout=5000;");    // wait up to 5s before giving up
}

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.MapGet("/api/tasks", async (AppDbContext db) =>
    await db.Tasks.OrderByDescending(t => t.CreatedAtUtc).ToListAsync());

app.MapGet("/api/tasks/{id:int}", async (int id, AppDbContext db) =>
    await db.Tasks.FindAsync(id) is { } t ? Results.Ok(t) : Results.NotFound());

app.MapPost("/api/tasks", async (TaskDto dto, AppDbContext db) =>
{
    var item = new TaskItem { Title = dto.Title, Description = dto.Description };
    db.Tasks.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tasks/{item.Id}", item);
});

app.MapPut("/api/tasks/{id:int}", async (int id, TaskDto dto, AppDbContext db) =>
{
    var existing = await db.Tasks.FindAsync(id);
    if (existing is null) return Results.NotFound();
    existing.Title = dto.Title;
    existing.Description = dto.Description;
    await db.SaveChangesAsync();
    return Results.Ok(existing);
});

app.MapDelete("/api/tasks/{id:int}", async (int id, AppDbContext db) =>
{
    var existing = await db.Tasks.FindAsync(id);
    if (existing is null) return Results.NotFound();
    db.Remove(existing);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

public record TaskDto(string Title, string? Description);
public partial class Program { } // for WebApplicationFactory