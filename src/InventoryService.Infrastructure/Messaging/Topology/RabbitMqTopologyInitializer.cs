
using InventoryService.Infrastructure.Messaging.Configurations;
using InventoryService.Infrastructure.Messaging.Connections;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace InventoryService.Infrastructure.Messaging.Topology;

public class RabbitMqTopologyInitializer : IRabbitMqTopologyInitializer
{
    private readonly RabbitMqConnection _connection;
    private readonly RabbitMqOptions _options;
    public RabbitMqTopologyInitializer(RabbitMqConnection connection, IOptions<RabbitMqOptions> options)
    {
        _connection = connection;
        _options = options.Value;
    }
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var channel = await _connection.CreateChannelAsync(cancellationToken);

        //ensure this exchange exists and re-used
        await channel.ExchangeDeclareAsync(
            exchange: _options.Exchange,
            type: _options.ExchangeType,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: QueueNames.InventoryReservation,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: QueueNames.InventoryReservation,
            exchange: _options.Exchange,
            routingKey: RoutingKeys.OrderCreated);
    }
}