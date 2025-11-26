using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using Reviews.Api.Data;
using Reviews.Api.Models;

namespace Reviews.Api.GraphQL;

public class Queries
{
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Review> GetReviews([Service] IDbContextFactory<AppDbContext> contextFactory)
    {
        using var context = contextFactory.CreateDbContext();
        return context.Reviews.AsNoTracking();
    }
}
