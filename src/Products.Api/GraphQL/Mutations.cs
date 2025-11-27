using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Products.Api.Data;
using Products.Api.Models;

namespace Products.Api.GraphQL;

public class Mutations
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ILogger<Mutations> _logger;

    public Mutations(IDbContextFactory<AppDbContext> contextFactory, ILogger<Mutations> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<AddProductPayload> AddProduct(
        AddProductInput input,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        using var context = _contextFactory.CreateDbContext();
        
        var product = new Product
        {
            Name = input.Name,
            Description = input.Description,
            Price = input.Price,
            CategoryId = input.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        await eventSender.SendAsync(nameof(Subscriptions.ProductAdded), product, cancellationToken);

        return new AddProductPayload(product);
    }

    public async Task<UpdateProductPayload> UpdateProduct(
        UpdateProductInput input,
        CancellationToken cancellationToken)
    {
        using var context = _contextFactory.CreateDbContext();
        
        var product = await context.Products.FindAsync(new object[] { input.Id }, cancellationToken);
        if (product is null)
        {
            _logger.LogError("Product with ID {ProductId} not found for update", input.Id);
            throw new Exception($"Product with ID {input.Id} not found");
        }

        if (!string.IsNullOrEmpty(input.Name))
            product.Name = input.Name;
        if (!string.IsNullOrEmpty(input.Description))
            product.Description = input.Description;
        if (input.Price.HasValue)
            product.Price = input.Price.Value;
        if (input.CategoryId.HasValue)
            product.CategoryId = input.CategoryId.Value;

        await context.SaveChangesAsync(cancellationToken);

        return new UpdateProductPayload(product);
    }

    public async Task<DeleteProductPayload> DeleteProduct(
        int id,
        [Service] IDbContextFactory<AppDbContext> contextFactory,
        CancellationToken cancellationToken)
    {
        using var context = contextFactory.CreateDbContext();
        
        var product = await context.Products.FindAsync(new object[] { id }, cancellationToken);
        if (product is null)
        {
            _logger.LogError("Product with ID {ProductId} not found for deletion", id);
            throw new Exception($"Product with ID {id} not found");
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);

        return new DeleteProductPayload(id, true);
    }
}

public record AddProductInput(string Name, string Description, decimal Price, int CategoryId);
public record AddProductPayload(Product Product);

public record UpdateProductInput(int Id, string? Name, string? Description, decimal? Price, int? CategoryId);
public record UpdateProductPayload(Product Product);

public record DeleteProductPayload(int Id, bool Success);
