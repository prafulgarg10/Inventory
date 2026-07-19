using InventoryService.Domain.Events;

namespace InventoryService.Infrastructure.Messaging.Routing;
public static class RoutingKeyMapper
{
    public static string GetRoutingKey(IIntegrationEvent integrationEvent)
    {
        return integrationEvent switch
        {
            InventoryReservedEvent => "inventory.reserved",
            InventoryReservedFailedEvent => "inventory.reserved.failed",
            _ => throw new NotSupportedException($"No routing key configured for {integrationEvent.GetType().Name}")
        };
    }
}