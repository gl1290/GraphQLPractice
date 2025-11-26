using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using Products.Api.Data;
using Products.Api.Models;

namespace Products.Api.GraphQL;

public class Queries
{
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Product> GetProducts([Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        using var context = contextFactory.CreateDbContext();
        return context.Products.AsNoTracking();
    }

    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Category> GetCategories([Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        using var context = contextFactory.CreateDbContext();
        return context.Categories.AsNoTracking();
    }
}
