
using InventoryService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

public class ProcessedMessageRepository : IProcessedMessageRepository
{
    private readonly AppDbContext _dbContext;
    public ProcessedMessageRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken)
    {
        await _dbContext.ProcessedMessages.AddAsync(processedMessage, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid messageId, CancellationToken cancellationToken)
    {
        return await _dbContext.ProcessedMessages.AnyAsync(pm => pm.MessageId == messageId, cancellationToken);
    }
}