using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PrintHub.Application.Features.Orders;
using PrintHub.Contracts.Messaging;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;
using PrintHub.Infrastructure.Messaging;
using PrintHub.Infrastructure.Persistence;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PrintHub.ProductionAgent;

/// <summary>
/// Consumes production jobs from RabbitMQ and drives each order through the
/// production stage: it simulates the machine run, then transitions the order
/// InProduction → ReadyForPickup / OutForDelivery as the System actor and appends
/// the transition to the order history — the same state machine the API uses.
/// Processing one message at a time (BasicQos = 1) with manual acknowledgement so a
/// crash re-queues the job rather than losing it.
/// </summary>
public class ProductionWorker : BackgroundService
{
    private static readonly TimeSpan SimulatedProduction = TimeSpan.FromSeconds(4);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<ProductionWorker> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    public ProductionWorker(IServiceScopeFactory scopeFactory, IOptions<RabbitMqOptions> options, ILogger<ProductionWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConnectWithRetryAsync(stoppingToken);
        if (stoppingToken.IsCancellationRequested || _channel is null) return;

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, ea) => Handle(ea, stoppingToken);
        _channel.BasicConsume(ProductionTopology.QueueName, autoAck: false, consumer);
        _logger.LogInformation("Production agent listening on '{Queue}'.", ProductionTopology.QueueName);

        try { await Task.Delay(Timeout.Infinite, stoppingToken); }
        catch (OperationCanceledException) { /* shutting down */ }
    }

    private void Handle(BasicDeliverEventArgs ea, CancellationToken ct)
    {
        try
        {
            var message = JsonSerializer.Deserialize<ProductionJobMessage>(ea.Body.Span);
            if (message is not null)
                ProcessAsync(message, ct).GetAwaiter().GetResult();
            _channel!.BasicAck(ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process a production message; not requeuing.");
            try { _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false); } catch { /* ignore */ }
        }
    }

    private async Task ProcessAsync(ProductionJobMessage message, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PrintHubDbContext>();

        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == message.OrderId, ct);
        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found; skipping.", message.OrderId);
            return;
        }
        if (order.Status != OrderStatus.InProduction)
        {
            _logger.LogInformation("Order {OrderCode} is {Status}, not InProduction; skipping.", order.OrderCode, order.Status);
            return;
        }

        _logger.LogInformation("Producing {OrderCode}...", order.OrderCode);
        await Task.Delay(SimulatedProduction, ct);

        var target = order.FulfilmentMethod == FulfilmentMethod.Delivery
            ? OrderStatus.OutForDelivery
            : OrderStatus.ReadyForPickup;

        if (!OrderStateMachine.CanTransition(order.Status, target, OrderActor.System))
        {
            _logger.LogWarning("Cannot move {OrderCode} from {From} to {Target}.", order.OrderCode, order.Status, target);
            return;
        }

        var from = order.Status;
        order.Status = target;
        order.ProgressPercent = 100;
        db.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            FromStatus = from,
            ToStatus = target,
            ActorUserId = null,
            ActorRole = null,
            Reason = "Production completed by agent.",
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync(ct);

        _logger.LogInformation("{OrderCode}: {From} -> {Target}.", order.OrderCode, from, target);
    }

    private async Task ConnectWithRetryAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = false
        };

        while (!ct.IsCancellationRequested)
        {
            try
            {
                _connection = factory.CreateConnection("printhub-production-agent");
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(ProductionTopology.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                _logger.LogInformation("Connected to RabbitMQ at {Host}:{Port}.", _options.HostName, _options.Port);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RabbitMQ not reachable; retrying in 5s...");
                try { await Task.Delay(TimeSpan.FromSeconds(5), ct); }
                catch (OperationCanceledException) { return; }
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try { _channel?.Close(); } catch { /* ignore */ }
        try { _connection?.Close(); } catch { /* ignore */ }
        await base.StopAsync(cancellationToken);
    }
}
