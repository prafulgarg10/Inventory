using RabbitMQ.Client;

namespace InventoryService.Infrastructure.Messaging.Publisher;

public interface IPublisherConfirmationAwaiter
{
    Task ExecuteConfirmationAsync(Func<Task> publishOperation, IChannel channel, CancellationToken cancellationToken);
}