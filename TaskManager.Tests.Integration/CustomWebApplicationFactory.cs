using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Api.Data;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _conn;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            services.Remove(descriptor);

            // Create single shared in-memory SQLite connection
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_conn));

            // Build provider, ensure DB
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _conn?.Dispose();
    }
}
