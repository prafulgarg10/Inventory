using InventoryService.Application.Interfaces;
using InventoryService.Infrastructure.BackgroundServices;
using InventoryService.Infrastructure.Messaging.Configurations;
using InventoryService.Infrastructure.Messaging.Connections;
using InventoryService.Infrastructure.Messaging.Publisher;
using InventoryService.Infrastructure.Messaging.Topology;
using InventoryService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.Infrastructure;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DBConnection"));
        });
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<IProcessedMessageRepository, ProcessedMessageRepository>();
        services.AddScoped<IUnitofWorkRepository, UnitofWorkRepository>();

        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.AddSingleton<RabbitMqConnection>();
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
        services.AddSingleton<IRabbitMqTopologyInitializer, RabbitMqTopologyInitializer>();
        services.AddSingleton<IPublisherConfirmationAwaiter, PublisherConfirmationAwaiter>();

        services.AddHostedService<OutboxPublisherBackgroundService>();
        services.AddHostedService<ReserveInventoryConsumer>();
        return services;
    }
}