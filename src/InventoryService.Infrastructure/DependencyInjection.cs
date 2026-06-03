using InventoryService.Application.Common.Interfaces;
using InventoryService.Application.Products.Interfaces;
using InventoryService.Infrastructure.Persistence;
using InventoryService.Infrastructure.Repositories;
using InventoryService.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("InventoryDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'InventoryDatabase' is not configured.");
        }

        services.AddDbContext<InventoryDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}