using HotChocolate.Data;
using Products.Api.Data;
using Products.Api.Models;

namespace Products.Api.GraphQL;

public class Queries
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Product> GetProducts([Service] AppDbContext context)
    {
        return context.Products;
    }

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Category> GetCategories([Service] AppDbContext context)
    {
        return context.Categories;
    }
}
