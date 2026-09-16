// Api/Extensions/SeedAdmin.cs
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Api.Extensions;

public static class SeedAdmin
{
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!await db.Users.AnyAsync(u => u.Role == "Admin"))
        {
            db.Users.Add(new User
            {
                Username = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }
    }
}