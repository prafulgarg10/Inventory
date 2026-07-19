namespace InventoryService.Application.Interfaces;
public interface IProcessedMessageRepository
{
    Task AddAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid messageId, CancellationToken cancellationToken);
}