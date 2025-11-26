using System.Text;
using System.Text.Json;

namespace Gateway.Extensions;

public class ProductReviewsTypeExtension
{
    [GraphQLType("Product")]
    public static async Task<ReviewsPaged?> GetReviews(
        [Parent] ProductData product,
        [Service] IHttpClientFactory httpClientFactory,
        int? first = null,
        CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("reviews");

        var query = first.HasValue 
            ? $@"{{ reviews(where: {{ productId: {{ eq: {product.Id} }} }}, first: {first.Value}) {{ nodes {{ id productId author comment rating createdAt }} totalCount }} }}"
            : $@"{{ reviews(where: {{ productId: {{ eq: {product.Id} }} }}) {{ nodes {{ id productId author comment rating createdAt }} totalCount }} }}";

        var request = new
        {
            query = query
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<GraphQLResponse>(responseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (result?.Data?.Reviews == null)
        {
            return null;
        }

        return new ReviewsPaged(
            result.Data.Reviews.Nodes?.Select(r => new GatewayReview(
                r.Id,
                r.ProductId,
                r.Author ?? string.Empty,
                r.Comment ?? string.Empty,
                r.Rating,
                r.CreatedAt
            )).ToList() ?? new List<GatewayReview>(),
            result.Data.Reviews.TotalCount ?? 0
        );
    }
}

public record ProductData(int Id, string Name, string Description, decimal Price, int CategoryId, DateTime CreatedAt);

public record GatewayReview(int Id, int ProductId, string Author, string Comment, int Rating, DateTime CreatedAt);

public record ReviewsPaged(List<GatewayReview> Nodes, int TotalCount);

public class GraphQLResponse
{
    public ResponseData? Data { get; set; }
}

public class ResponseData
{
    public ReviewsData? Reviews { get; set; }
}

public class ReviewsData
{
    public List<ReviewNode>? Nodes { get; set; }
    public int? TotalCount { get; set; }
}

public class ReviewNode
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? Author { get; set; }
    public string? Comment { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}
