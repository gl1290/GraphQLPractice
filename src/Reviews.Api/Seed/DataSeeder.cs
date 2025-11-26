using Microsoft.EntityFrameworkCore;
using Reviews.Api.Data;
using Reviews.Api.Models;

namespace Reviews.Api.Seed;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IDbContextFactory<AppDbContext> contextFactory)
    {
        await using var context = contextFactory.CreateDbContext();
        
        await context.Database.EnsureCreatedAsync();
        
        if (await context.Reviews.AnyAsync())
        {
            return; // Already seeded
        }

        var reviews = new[]
        {
            new Review { ProductId = 1, Author = "John Doe", Comment = "Excellent laptop, very fast!", Rating = 5, CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new Review { ProductId = 1, Author = "Jane Smith", Comment = "Good performance but a bit pricey", Rating = 4, CreatedAt = DateTime.UtcNow.AddDays(-8) },
            new Review { ProductId = 2, Author = "Bob Wilson", Comment = "Amazing phone with great camera", Rating = 5, CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new Review { ProductId = 3, Author = "Alice Brown", Comment = "Best headphones I've ever owned", Rating = 5, CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new Review { ProductId = 4, Author = "Charlie Davis", Comment = "A timeless classic", Rating = 5, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Review { ProductId = 5, Author = "Diana Evans", Comment = "Thought-provoking and relevant", Rating = 5, CreatedAt = DateTime.UtcNow.AddDays(-1) }
        };

        await context.Reviews.AddRangeAsync(reviews);
        await context.SaveChangesAsync();
    }
}
