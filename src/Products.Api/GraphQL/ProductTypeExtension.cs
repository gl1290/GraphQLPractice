using Products.Api.Models;

namespace Products.Api.GraphQL;

[ExtendObjectType(typeof(Product))]
public class ProductTypeExtension
{
    public async Task<Category?> GetCategory(
        [Parent] Product product,
        CategoryDataLoader categoryLoader,
        CancellationToken cancellationToken)
    {
        return await categoryLoader.LoadAsync(product.CategoryId, cancellationToken);
    }
}
