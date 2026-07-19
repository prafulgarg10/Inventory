using InventoryService.Infrastructure.Messaging.Publisher;
using InventoryService.Infrastructure.Messaging.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InventoryService.Infrastructure.BackgroundServices;
public sealed class OutboxPublisherBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisherBackgroundService> _logger;
    private readonly IMessagePublisher _publisher;
    public OutboxPublisherBackgroundService(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisherBackgroundService> logger, IMessagePublisher publisher)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _publisher = publisher;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublishPendingMessagesAsync(stoppingToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while publishing outbox messages");
            }
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task PublishPendingMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var pendingMessages = await dbContext.OutboxMessages.Where(o => !o.IsProcessed).OrderBy(o => o.CreatedAt).Take(50).ToListAsync(cancellationToken);
        foreach (var message in pendingMessages)
        {
            await _publisher.PublishAsync(RoutingKeyMapper.Get(message.EventType), message.Payload, cancellationToken);
            //mark the message as processed.
            message.MarkAsProcessed();
        }

        //if no error then save the changes.
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}