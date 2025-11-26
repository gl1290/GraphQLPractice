using Microsoft.EntityFrameworkCore;
using Reviews.Api.Data;
using Reviews.Api.GraphQL;
using Reviews.Api.Seed;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 5002
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5002);
});

// Add DbContext with SQLite
builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseSqlite("Data Source=reviews.db"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=reviews.db"));

// Add GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Queries>()
    .AddMutationType<Mutations>()
    .AddFiltering()
    .AddSorting()
    .AddProjections();

var app = builder.Build();

// Seed data
var contextFactory = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
await DataSeeder.SeedDataAsync(contextFactory);

app.MapGraphQL();

app.Run();
