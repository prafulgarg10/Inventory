using InventoryService.Infrastructure.Messaging.Configurations;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace InventoryService.Infrastructure.Messaging.Connections;
public sealed class RabbitMqConnection : IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly SemaphoreSlim _lock = new(1,1);
    private IConnection? _connection;
    private readonly ConnectionFactory _factory;
    public RabbitMqConnection(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
        _factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password
            };
    }
    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        await EnsureConnectionAsync(cancellationToken);
        return await _connection!.CreateChannelAsync(cancellationToken : cancellationToken);
    }

    private async Task EnsureConnectionAsync(CancellationToken cancellationToken)
    {
        if(_connection is {IsOpen: true})
            return;
            
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if(_connection is {IsOpen: true})
                return;
            
            if(_connection is not null)
            {
                await _connection.DisposeAsync();
            }

            _connection = await _factory.CreateConnectionAsync(cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if(_connection is not null)
        {
            await _connection.DisposeAsync();
        }
        _lock.Dispose();
    }
}