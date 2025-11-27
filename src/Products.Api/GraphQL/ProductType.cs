using HotChocolate.Types;
using Products.Api.Models;

namespace Products.Api.GraphQL;

public class ProductType : ObjectType<Product>
{
    protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
    {
        descriptor.Description("Product GraphQL type with computed fields and resolved category.");

        // Hide raw CategoryId from the public schema (we expose `category` instead)
        descriptor.Field(p => p.CategoryId).Ignore();

        // Expose createdAt as a non-null DateTime
        descriptor.Field(p => p.CreatedAt)
            .Type<NonNullType<DateTimeType>>()
            .Name("createdAt")
            .Description("UTC timestamp when the product was created.");

        // Computed string price field
        descriptor.Field("priceFormatted")
            .Type<NonNullType<StringType>>()
            .ResolveWith<Resolvers>(r => r.GetPriceFormatted(default!))
            .Description("Price formatted as a currency string.");

        // Computed age-in-days field
        descriptor.Field("ageInDays")
            .Type<NonNullType<IntType>>()
            .ResolveWith<Resolvers>(r => r.GetAgeInDays(default!))
            .Description("Number of days since the product was created (UTC).");

        // Category resolved via the CategoryDataLoader
        descriptor.Field("category")
            .ResolveWith<Resolvers>(r => r.GetCategoryAsync(default!, default!, default))
            .Description("The category for this product.");
    }

    private class Resolvers
    {
        // DataLoader will be injected by HotChocolate
        public async Task<Category?> GetCategoryAsync(Product product, CategoryDataLoader loader, CancellationToken ct)
        {
            if (product is null) return null;
            return await loader.LoadAsync(product.CategoryId, ct);
        }

        public string GetPriceFormatted(Product product)
        {
            return product.Price.ToString("C");
        }

        public int GetAgeInDays(Product product)
        {
            return (int)(DateTime.UtcNow - product.CreatedAt).TotalDays;
        }
    }
}