using Microsoft.EntityFrameworkCore;

namespace Products.Api.Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Register additional services here as needed in the future
        services.AddDbContextFactories();

        return services;
    }

    private static IServiceCollection AddDbContextFactories(this IServiceCollection services)
    {
        services.AddPooledDbContextFactory<Data.AppDbContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlite("Data Source=products.db");
        });
        return services;
    }
}