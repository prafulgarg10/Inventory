using System.Text.Json;
using InventoryService.Infrastructure.Messaging.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InventoryService.Infrastructure.BackgroundServices;

public sealed class ReserveInventoryConsumer : BackgroundService
{
    private readonly ILogger<ReserveInventoryConsumer> _logger;
    private readonly RabbitMqConnection _connection;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public ReserveInventoryConsumer(ILogger<ReserveInventoryConsumer> logger, RabbitMqConnection connection, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _connection = connection;
        _serviceScopeFactory = serviceScopeFactory;
    }
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var channel = await _connection.CreateChannelAsync(stoppingToken);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async(_, args) =>
        {
            await HandleMessageAsync(channel, args, stoppingToken);
        };

        //this will let rabbitmq send only one message at a time for ack.
        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken
        );

        await channel.BasicConsumeAsync(
            queue: QueueNames.InventoryReservation,
            autoAck: false,
            consumer: consumer,
            consumerTag: "inventory.reserve.consumer",
            noLocal: false,
            exclusive: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessageAsync(IChannel channel, BasicDeliverEventArgs args, CancellationToken stoppingToken)
    {
        try
        {
            var json = System.Text.Encoding.UTF8.GetString(args.Body.Span);
            var evt = JsonSerializer.Deserialize<OrderCreatedIntegrationEvent>(json);
            if(evt is null)
            {
                _logger.LogWarning("Unable to deserialize message. DeliveryTag: {DeliveryTag}", args.DeliveryTag);
                throw new JsonException("Unable to deserialize message.");
            }
            using var scope = _serviceScopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<ReserveInventoryCommandHandler>();
            var command = new ReserveInventoryCommand(
                evt.MessageId,
                evt.OrderNumber,
                evt.CustomerId,
                evt.Amount,
                evt.Items.Select(i => new ReserveInventoryItem(i.ProductId, i.Quantity)).ToList()
            );
            await handler.HandleAsync(command, stoppingToken);
            //no exception means message is processed successfully, so we can acknowledge the message.
            await channel.BasicAckAsync(args.DeliveryTag, false, stoppingToken);
        }
        catch(JsonException ex)
        {
            _logger.LogError(ex, "Invalid message.");
            //we don't want to requeue the message as it is invalid, so we nack it without requeueing.
            await channel.BasicAckAsync(args.DeliveryTag, false, stoppingToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error processing inventory reservation.");
            await channel.BasicNackAsync(args.DeliveryTag, false, true, stoppingToken);
        }
    }
}