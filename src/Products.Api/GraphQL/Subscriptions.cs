using Products.Api.Models;

namespace Products.Api.GraphQL;

public class Subscriptions
{
    [Subscribe]
    [Topic(nameof(ProductAdded))]
    public Product ProductAdded([EventMessage] Product product) => product;
}
