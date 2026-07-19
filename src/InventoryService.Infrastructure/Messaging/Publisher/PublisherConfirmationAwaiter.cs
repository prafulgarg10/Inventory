using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InventoryService.Infrastructure.Messaging.Publisher;

public sealed class PublisherConfirmationAwaiter : IPublisherConfirmationAwaiter
{    
    public async Task ExecuteConfirmationAsync(Func<Task> publishOperation, IChannel channel, CancellationToken cancellationToken)
    {
        //Required new for every call
        var confirmation = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        AsyncEventHandler<BasicAckEventArgs> ackHandler = async (_, args) =>
        {
            confirmation.TrySetResult();
            await Task.CompletedTask;
        };   

        AsyncEventHandler<BasicNackEventArgs> nackHandler = async (_, args) =>
        {
            confirmation.TrySetException(
                new InvalidOperationException("RabbitMq negatively acknowledged publish.")
            );

            await Task.CompletedTask;
        };

        AsyncEventHandler<BasicReturnEventArgs> basicReturnHandler = async (_, args) =>
        {
            confirmation.TrySetException(
                new InvalidOperationException($"Message returned. RoutingKey={args.RoutingKey}")
            );
            
            await Task.CompletedTask;
        };

        channel.BasicAcksAsync += ackHandler;
        channel.BasicNacksAsync += nackHandler;
        // no queue available to handle the message
        channel.BasicReturnAsync += basicReturnHandler;

        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
            await publishOperation();
            //this will keep waiting for the confirmation task to be released. Added linked token to handle timeout.
            //If RabbitMq take longer then we will discard it to prevent from infinite waiting
            await confirmation.Task.WaitAsync(linked.Token);
        }
        finally
        {
            channel.BasicAcksAsync -= ackHandler;
            channel.BasicNacksAsync -= nackHandler;
            channel.BasicReturnAsync -= basicReturnHandler;
        }        
    }
}