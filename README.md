# GraphQL EF Core Sample with Schema Stitching

A complete demonstration of GraphQL APIs built with .NET 7, Entity Framework Core, HotChocolate, and schema stitching gateway.

## Architecture

This solution consists of three microservices:

- **Products.Api** (Port 5001): Manages products and categories with full CRUD operations, subscriptions, and data loaders
- **Reviews.Api** (Port 5002): Manages product reviews
- **Gateway** (Port 5000): Schema stitching gateway that combines Products and Reviews APIs

## Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Node.js 18+](https://nodejs.org/) (for client demo)
- [Docker](https://www.docker.com/) and Docker Compose (optional, for containerized deployment)

## Project Structure

```
GraphQLSample/
├── src/
│   ├── Products.Api/          # Products microservice
│   │   ├── Models/            # Product and Category models
│   │   ├── Data/              # EF Core DbContext
│   │   ├── GraphQL/           # GraphQL queries, mutations, subscriptions, data loaders
│   │   ├── Seed/              # Database seeder
│   │   └── Dockerfile
│   ├── Reviews.Api/           # Reviews microservice
│   │   ├── Models/            # Review model
│   │   ├── Data/              # EF Core DbContext
│   │   ├── GraphQL/           # GraphQL queries and mutations
│   │   ├── Seed/              # Database seeder
│   │   └── Dockerfile
│   ├── Gateway/               # Schema stitching gateway
│   │   ├── Extensions/        # Type extensions for stitched schema
│   │   └── Dockerfile
│   └── Client/                # Node.js GraphQL client demo
│       ├── package.json
│       └── client.js
├── GraphQLSample.sln
├── docker-compose.yml
└── README.md
```

## Running Locally

### Option 1: Run with .NET CLI

1. **Restore dependencies and build the solution:**
   ```bash
   dotnet restore
   dotnet build
   ```

2. **Run each service in a separate terminal:**

   Terminal 1 - Products API:
   ```bash
   cd src/Products.Api
   dotnet run
   ```

   Terminal 2 - Reviews API:
   ```bash
   cd src/Reviews.Api
   dotnet run
   ```

   Terminal 3 - Gateway:
   ```bash
   cd src/Gateway
   dotnet run
   ```

3. **Access the GraphQL endpoints:**
   - Products API: http://localhost:5001/graphql
   - Reviews API: http://localhost:5002/graphql
   - Gateway (Stitched): http://localhost:5000/graphql

### Option 2: Run with Docker Compose

1. **Build and start all services:**
   ```bash
   docker-compose up --build
   ```

2. **Access the Gateway:**
   - Gateway: http://localhost:5000/graphql

3. **Stop all services:**
   ```bash
   docker-compose down
   ```

## Running the Client Demo

1. **Install Node.js dependencies:**
   ```bash
   cd src/Client
   npm install
   ```

2. **Run the client:**
   ```bash
   npm start
   ```

The client demonstrates:
- Querying products with category information
- Querying products with reviews (via stitched schema)
- Adding reviews through mutations
- Subscribing to product additions (optional)

## GraphQL Examples

### Query: Get Products with Categories

```graphql
query {
  products(first: 5) {
    nodes {
      id
      name
      description
      price
      category {
        name
        description
      }
    }
    totalCount
  }
}
```

### Query: Get Products with Reviews (via Gateway)

```graphql
query {
  products(where: { id: { eq: 1 } }) {
    nodes {
      id
      name
      price
      reviews(first: 10) {
        nodes {
          id
          author
          comment
          rating
          createdAt
        }
        totalCount
      }
    }
  }
}
```

### Query: Get All Reviews

```graphql
query {
  reviews(first: 10, order: { createdAt: DESC }) {
    nodes {
      id
      productId
      author
      comment
      rating
      createdAt
    }
    totalCount
  }
}
```

### Mutation: Add a Product

```graphql
mutation {
  addProduct(input: {
    name: "Wireless Mouse"
    description: "Ergonomic wireless mouse"
    price: 29.99
    categoryId: 1
  }) {
    product {
      id
      name
      price
      createdAt
    }
  }
}
```

### Mutation: Update a Product

```graphql
mutation {
  updateProduct(input: {
    id: 1
    name: "Updated Laptop Name"
    price: 1199.99
  }) {
    product {
      id
      name
      price
    }
  }
}
```

### Mutation: Add a Review

```graphql
mutation {
  addReview(input: {
    productId: 1
    author: "John Doe"
    comment: "Great product, highly recommended!"
    rating: 5
  }) {
    review {
      id
      author
      comment
      rating
      createdAt
    }
  }
}
```

### Subscription: Listen for New Products

```graphql
subscription {
  productAdded {
    id
    name
    description
    price
    createdAt
  }
}
```

**Note:** To test subscriptions, connect to the Products API WebSocket endpoint at `ws://localhost:5001/graphql` and run the subscription. In another window, add a product via mutation to trigger the subscription.

## Advanced Querying

### Filtering Products by Price

```graphql
query {
  products(where: { price: { gte: 100, lte: 1000 } }) {
    nodes {
      name
      price
    }
  }
}
```

### Sorting Products

```graphql
query {
  products(order: { price: DESC }) {
    nodes {
      name
      price
    }
  }
}
```

### Filtering Reviews by Rating

```graphql
query {
  reviews(where: { rating: { gte: 4 } }) {
    nodes {
      author
      comment
      rating
    }
  }
}
```

## Database

Both Products.Api and Reviews.Api use SQLite databases:
- `products.db` - Created in Products.Api directory
- `reviews.db` - Created in Reviews.Api directory

The databases are automatically created and seeded with sample data on first run.

## Technology Stack

- **.NET 7.0**: Target framework
- **HotChocolate 13.9.0**: GraphQL server implementation
- **Entity Framework Core 7.0.20**: ORM with SQLite provider
- **HotChocolate.Data**: Filtering, sorting, and pagination
- **HotChocolate.Subscriptions**: Real-time subscriptions with in-memory event bus
- **HotChocolate.Stitching**: Schema stitching for gateway
- **graphql-request**: GraphQL client library (Node.js)
- **graphql-ws**: WebSocket client for subscriptions (Node.js)

## Features Demonstrated

### Products.Api
- ✅ Entity Framework Core with SQLite
- ✅ GraphQL Queries with filtering, sorting, and pagination
- ✅ GraphQL Mutations (Add, Update, Delete)
- ✅ GraphQL Subscriptions (Real-time product additions)
- ✅ DataLoader pattern for efficient batch loading (Categories)
- ✅ Type extensions (Product → Category relationship)
- ✅ Automatic database migration and seeding

### Reviews.Api
- ✅ GraphQL Queries with filtering, sorting, and pagination
- ✅ GraphQL Mutations (Add reviews)
- ✅ Automatic database migration and seeding

### Gateway
- ✅ Schema stitching from multiple downstream services
- ✅ Type extension to add reviews to Product type
- ✅ HTTP-based GraphQL communication with downstream services
- ✅ Environment-based service URL configuration

### Client
- ✅ Query execution with graphql-request
- ✅ Mutation execution
- ✅ WebSocket subscriptions with graphql-ws

## Development

### Adding New Fields

To add fields to existing types:

1. Update the model in the respective API project
2. Update the database (EF Core will handle this on next run with SQLite)
3. The GraphQL schema will automatically include the new fields

### Adding New Services

To add a new microservice to the gateway:

1. Create the new service following the existing pattern
2. Add an HttpClient in Gateway's Program.cs
3. Add the remote schema: `.AddRemoteSchema("serviceName")`
4. Create type extensions if needed for cross-service relationships

## Troubleshooting

### Port Already in Use

If you get port conflicts, you can change the ports in:
- `src/Products.Api/Program.cs` (currently 5001)
- `src/Reviews.Api/Program.cs` (currently 5002)
- `src/Gateway/Program.cs` (currently 5000)
- `docker-compose.yml` (for containerized deployment)

### Database Issues

If you encounter database issues:
```bash
# Delete the databases and restart
rm src/Products.Api/products.db*
rm src/Reviews.Api/reviews.db*
```

The databases will be recreated and reseeded on next run.

### Docker Build Issues

If Docker builds fail:
```bash
# Clean Docker cache
docker-compose down
docker system prune -a
docker-compose up --build
```

## Learning Resources

- [HotChocolate Documentation](https://chillicream.com/docs/hotchocolate)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [GraphQL Specification](https://spec.graphql.org/)
- [Schema Stitching Guide](https://chillicream.com/docs/hotchocolate/distributed-schema/schema-stitching)

## License

This is a sample project for educational purposes.
