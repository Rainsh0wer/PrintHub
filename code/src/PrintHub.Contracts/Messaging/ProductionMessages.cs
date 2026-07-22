namespace PrintHub.Contracts.Messaging;

/// <summary>
/// Message published when a shop starts production, consumed asynchronously by the
/// Production Agent. Kept minimal: the agent reloads the order from the database by
/// id, so the message only needs to identify the job.
/// </summary>
public record ProductionJobMessage(int OrderId, string OrderCode, DateTime EnqueuedAt);

/// <summary>Shared messaging topology constants for the production pipeline.</summary>
public static class ProductionTopology
{
    /// <summary>Durable work queue the API publishes to and the agent consumes from.</summary>
    public const string QueueName = "printhub.production";
}
