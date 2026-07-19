using InventoryService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

public class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly AppDbContext _dbContext;

    public OutboxMessageRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    }

    public async Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize, CancellationToken cancellationToken)
    {
        return await _dbContext.OutboxMessages
            .Where(m => !m.IsProcessed)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }
}