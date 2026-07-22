namespace PrintHub.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the async production pipeline. The Application layer enqueues a
/// job when a shop starts production; the Infrastructure implementation publishes it
/// to the broker. Implementations degrade gracefully — if the broker is unreachable
/// the enqueue is a no-op, and the order can still be driven manually — so ordering
/// never fails just because messaging is down.
/// </summary>
public interface IProductionQueue
{
    Task EnqueueProductionAsync(int orderId, string orderCode, CancellationToken ct = default);
}
