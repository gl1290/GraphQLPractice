using Gateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 5000
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

// Add HTTP clients for downstream services
builder.Services.AddHttpClient("products", client =>
{
    var productsUrl = Environment.GetEnvironmentVariable("PRODUCTS_URL") ?? "http://localhost:5001/graphql";
    client.BaseAddress = new Uri(productsUrl);
});

builder.Services.AddHttpClient("reviews", client =>
{
    var reviewsUrl = Environment.GetEnvironmentVariable("REVIEWS_URL") ?? "http://localhost:5002/graphql";
    client.BaseAddress = new Uri(reviewsUrl);
});

// Add GraphQL with schema stitching
builder.Services
    .AddGraphQLServer()
    .AddRemoteSchema("products", ignoreRootTypes: true)
    .AddRemoteSchema("reviews", ignoreRootTypes: true)
    .AddTypeExtension<ProductReviewsTypeExtension>();

var app = builder.Build();

app.MapGraphQL();

app.Run();
