using Microsoft.EntityFrameworkCore;
using Products.Api.Data;
using Products.Api.Models;

namespace Products.Api.Seed;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IDbContextFactory<AppDbContext> contextFactory)
    {
        await using var context = contextFactory.CreateDbContext();
        
        await context.Database.EnsureCreatedAsync();
        
        if (await context.Categories.AnyAsync())
        {
            return; // Already seeded
        }

        var categories = new[]
        {
            new Category { Name = "Electronics", Description = "Electronic devices and gadgets" },
            new Category { Name = "Books", Description = "Books and literature" },
            new Category { Name = "Clothing", Description = "Apparel and fashion items" },
            new Category { Name = "Home & Garden", Description = "Home improvement and gardening" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        var products = new[]
        {
            new Product { Name = "Laptop", Description = "High-performance laptop", Price = 1299.99m, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Name = "Smartphone", Description = "Latest smartphone model", Price = 799.99m, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Name = "Headphones", Description = "Noise-cancelling headphones", Price = 199.99m, CategoryId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Name = "The Great Gatsby", Description = "Classic American novel", Price = 12.99m, CategoryId = 2, CreatedAt = DateTime.UtcNow },
            new Product { Name = "1984", Description = "Dystopian social science fiction", Price = 14.99m, CategoryId = 2, CreatedAt = DateTime.UtcNow },
            new Product { Name = "T-Shirt", Description = "Cotton t-shirt", Price = 19.99m, CategoryId = 3, CreatedAt = DateTime.UtcNow },
            new Product { Name = "Jeans", Description = "Blue denim jeans", Price = 49.99m, CategoryId = 3, CreatedAt = DateTime.UtcNow },
            new Product { Name = "Garden Hose", Description = "50ft expandable garden hose", Price = 29.99m, CategoryId = 4, CreatedAt = DateTime.UtcNow }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
