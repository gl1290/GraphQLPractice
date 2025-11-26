using Microsoft.EntityFrameworkCore;
using Products.Api.Data;
using Products.Api.GraphQL;
using Products.Api.Seed;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 5001
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001);
});

// Add DbContext with SQLite
builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseSqlite("Data Source=products.db"));

// Add GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Queries>()
    .AddMutationType<Mutations>()
    .AddSubscriptionType<Subscriptions>()
    .AddTypeExtension<ProductTypeExtension>()
    .AddDataLoader<CategoryDataLoader>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddInMemorySubscriptions();

var app = builder.Build();

// Seed data
var contextFactory = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
await DataSeeder.SeedDataAsync(contextFactory);

app.MapGraphQL();

app.UseWebSockets();

app.Run();
