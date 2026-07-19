namespace InventoryService.Infrastructure.Messaging.Publisher;
public interface IMessagePublisher
{
    Task PublishAsync(string routingKey, string message, CancellationToken cancellationToken = default);
}