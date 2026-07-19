namespace InventoryService.Domain.Events;
public sealed record InventoryReservedFailedEvent : IIntegrationEvent
{
    public Guid MessageId { get; init; }
    public Guid OrderNumber { get; init; }
    public string Reason { get; init; } = String.Empty;
    public DateTime OccuredAt { get; init; } = DateTime.UtcNow;
}