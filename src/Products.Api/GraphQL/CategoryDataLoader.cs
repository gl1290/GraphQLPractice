using Microsoft.EntityFrameworkCore;
using Products.Api.Data;
using Products.Api.Models;

namespace Products.Api.GraphQL;

public class CategoryDataLoader : BatchDataLoader<int, Category>
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public CategoryDataLoader(
        IDbContextFactory<AppDbContext> contextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _contextFactory = contextFactory;
    }

    protected override async Task<IReadOnlyDictionary<int, Category>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.CreateDbContext();
        
        var categories = await context.Categories
            .Where(c => keys.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        return categories;
    }
}
