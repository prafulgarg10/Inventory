using InventoryService.Application.Commands.CreateProduct;
using InventoryService.Application.Queries.GetProducts;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.Application;

public static class ApplicationDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ReserveInventoryCommandHandler>();
        services.AddScoped<GetProductsHandler>();
        services.AddScoped<CreateProductCommandHandler>();
        return services;
    }
}