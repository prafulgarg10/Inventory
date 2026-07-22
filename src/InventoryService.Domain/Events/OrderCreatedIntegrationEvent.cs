public class OrderCreatedIntegrationEvent
{
    public Guid MessageId { get; init; }
    public Guid OrderNumber { get; init; }
    public int CustomerId { get; init; }
    public decimal Amount { get; init; }
    public List<OrderItemDto> Items { get; init; } = [];
}

public class OrderItemDto
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice {get; init;}
}