using InventoryService.Domain.Events;

namespace InventoryService.Infrastructure.Messaging.Routing;
public static class RoutingKeyMapper
{
    private static readonly Dictionary<string, string> _map = new()
    {
        [nameof(InventoryReservedEvent)] =
            RoutingKeys.InventoryReserved,

        [nameof(InventoryReservedFailedEvent)] =
            RoutingKeys.InventoryReservedFailed
    };

    public static string Get(string eventType)
    {
        if (!_map.TryGetValue(eventType, out var routingKey))
        {
            throw new InvalidOperationException(
                $"No routing key configured for event '{eventType}'.");
        }

        return routingKey;
    }
}