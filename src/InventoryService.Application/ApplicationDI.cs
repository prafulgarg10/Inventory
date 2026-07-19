using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.Application;

public static class ApplicationDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ReserveInventoryCommandHandler>();
        return services;
    }
}