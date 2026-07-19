namespace InventoryService.Domain.Events;
public sealed record InventoryReservedEvent : IIntegrationEvent
{
    public Guid MessageId {get; init; }
    public Guid OrderNumber { get; init; }
    public int CustomerId { get; init; }
    public decimal Amount { get; init; }
    public DateTime OccuredAt { get; init; } = DateTime.UtcNow;
}