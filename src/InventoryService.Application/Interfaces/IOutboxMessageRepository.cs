namespace InventoryService.Application.Interfaces;
public interface IOutboxMessageRepository
{
    Task AddAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken);
    Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize, CancellationToken cancellationToken);
}