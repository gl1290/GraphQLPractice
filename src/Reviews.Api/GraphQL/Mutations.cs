using Microsoft.EntityFrameworkCore;
using Reviews.Api.Data;
using Reviews.Api.Models;

namespace Reviews.Api.GraphQL;

public class Mutations
{
    public async Task<AddReviewPayload> AddReview(
        AddReviewInput input,
        [Service] IDbContextFactory<AppDbContext> contextFactory,
        CancellationToken cancellationToken)
    {
        using var context = contextFactory.CreateDbContext();
        
        var review = new Review
        {
            ProductId = input.ProductId,
            Author = input.Author,
            Comment = input.Comment,
            Rating = input.Rating,
            CreatedAt = DateTime.UtcNow
        };

        context.Reviews.Add(review);
        await context.SaveChangesAsync(cancellationToken);

        return new AddReviewPayload(review);
    }
}

public record AddReviewInput(int ProductId, string Author, string Comment, int Rating);
public record AddReviewPayload(Review Review);
