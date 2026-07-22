using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Contracts.Messaging;
using RabbitMQ.Client;

namespace PrintHub.Infrastructure.Messaging;

/// <summary>
/// Publishes production jobs to RabbitMQ. A single connection/channel is created
/// lazily and reused. All broker interaction is guarded: if the broker is down the
/// publish is logged and swallowed, so a shop can still start production (and drive
/// the order manually) even with messaging offline — graceful degradation.
/// </summary>
public sealed class RabbitMqProductionQueue : IProductionQueue, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqProductionQueue> _logger;
    private readonly object _gate = new();

    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqProductionQueue(IOptions<RabbitMqOptions> options, ILogger<RabbitMqProductionQueue> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task EnqueueProductionAsync(int orderId, string orderCode, CancellationToken ct = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Messaging disabled; skipping production enqueue for {OrderCode}.", orderCode);
            return Task.CompletedTask;
        }

        try
        {
            var channel = EnsureChannel();
            var message = new ProductionJobMessage(orderId, orderCode, DateTime.UtcNow);
            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            props.ContentType = "application/json";

            channel.BasicPublish(exchange: "", routingKey: ProductionTopology.QueueName, basicProperties: props, body: body);
            _logger.LogInformation("Published production job for {OrderCode} (order {OrderId}).", orderCode, orderId);
        }
        catch (Exception ex)
        {
            // Degrade gracefully: the order is already InProduction and can be driven manually.
            _logger.LogWarning(ex, "Could not publish production job for {OrderCode}; the broker may be unavailable.", orderCode);
            ResetChannel();
        }

        return Task.CompletedTask;
    }

    private IModel EnsureChannel()
    {
        lock (_gate)
        {
            if (_channel is { IsOpen: true }) return _channel;

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                DispatchConsumersAsync = false
            };
            _connection = factory.CreateConnection("printhub-api-publisher");
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(ProductionTopology.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            return _channel;
        }
    }

    private void ResetChannel()
    {
        lock (_gate)
        {
            try { _channel?.Dispose(); } catch { /* ignore */ }
            try { _connection?.Dispose(); } catch { /* ignore */ }
            _channel = null;
            _connection = null;
        }
    }

    public void Dispose() => ResetChannel();
}
