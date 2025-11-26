using HotChocolate.Data;
using Reviews.Api.Data;
using Reviews.Api.Models;

namespace Reviews.Api.GraphQL;

public class Queries
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Review> GetReviews([Service] AppDbContext context)
    {
        return context.Reviews;
    }
}
