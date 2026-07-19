public sealed record ReserveInventoryCommand(Guid MessageId, Guid OrderNumber, int CustomerId, decimal Amount, IReadOnlyCollection<ReserveInventoryItem> Items);

public sealed record ReserveInventoryItem(int ProductId, int Quantity);