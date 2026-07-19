namespace InventoryService.Infrastructure.Messaging.Topology;
public interface IRabbitMqTopologyInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}