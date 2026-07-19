
using System.Text;
using InventoryService.Infrastructure.Messaging.Configurations;
using InventoryService.Infrastructure.Messaging.Connections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace InventoryService.Infrastructure.Messaging.Publisher;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly RabbitMqConnection _connection;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly RabbitMqOptions _options;
    private readonly IPublisherConfirmationAwaiter _publisherConfirmationAwaiter;
    public RabbitMqPublisher(RabbitMqConnection connection, ILogger<RabbitMqPublisher> logger, IOptions<RabbitMqOptions> options, IPublisherConfirmationAwaiter publisherConfirmationAwaiter)
    {
        _connection = connection;
        _options = options.Value;
        _logger = logger;
        _publisherConfirmationAwaiter = publisherConfirmationAwaiter;
    }
    public async Task PublishAsync(string routingKey, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var channel = await _connection.CreateChannelAsync(cancellationToken);

            await _publisherConfirmationAwaiter.ExecuteConfirmationAsync(() => PublishInternalAsync(channel, routingKey, message, cancellationToken), channel, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message with routing key {routingKey}", routingKey);
            throw;
        }
    }

    private async Task PublishInternalAsync(IChannel channel, string routingKey, string message, CancellationToken cancellationToken = default)
    {
            var body = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await channel.BasicPublishAsync(
                exchange: _options.Exchange,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Published message with routing key {routingKey}", routingKey);
    }
}