using Microsoft.EntityFrameworkCore;
using Products.Api.Data;
using Products.Api.GraphQL;
using Products.Api.Seed;
using Products.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 5001
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001);
});

// Add GraphQL
builder.Services
    .AddServices()
    .AddGraphQLServer()
    .AddQueryType<Queries>()
    .AddMutationType<Mutations>()
    .AddSubscriptionType<Subscriptions>()
    .AddTypeExtension<ProductTypeExtension>()
    .AddDataLoader<CategoryDataLoader>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddInMemorySubscriptions()
    .AddType<ProductType>();

var app = builder.Build();

// Seed data
var contextFactory = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
await DataSeeder.SeedDataAsync(contextFactory);

app.MapGraphQL();

app.UseWebSockets();

app.Run();