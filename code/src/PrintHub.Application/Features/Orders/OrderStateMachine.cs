using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Orders;

/// <summary>Who is attempting an order transition — the axis the transition table is keyed on.</summary>
public enum OrderActor
{
    /// <summary>The order's own customer.</summary>
    Customer,
    /// <summary>The fulfilling shop (owner or active staff).</summary>
    Shop,
    /// <summary>An automated transition (e.g. the production agent, expiry sweeper).</summary>
    System
}

/// <summary>
/// The order lifecycle as an explicit, table-driven state machine. Every allowed
/// transition — and which actor may perform it — is declared once here, so the
/// services never hard-code ad-hoc status checks and the SRS "Order Status
/// Definition" has a single source of truth in code.
/// </summary>
public static class OrderStateMachine
{
    private sealed record Transition(OrderStatus From, OrderStatus To, params OrderActor[] Actors);

    private static readonly Transition[] Transitions =
    {
        // Placement puts an order straight into AwaitingAcceptance; Draft/Quoted are
        // pre-placement states kept for completeness.
        new(OrderStatus.AwaitingAcceptance, OrderStatus.Accepted,       OrderActor.Shop),
        new(OrderStatus.AwaitingAcceptance, OrderStatus.Declined,       OrderActor.Shop),
        new(OrderStatus.AwaitingAcceptance, OrderStatus.Cancelled,      OrderActor.Customer),

        new(OrderStatus.Accepted,           OrderStatus.InProduction,   OrderActor.Shop),
        new(OrderStatus.Accepted,           OrderStatus.Cancelled,      OrderActor.Customer),

        new(OrderStatus.InProduction,       OrderStatus.ReadyForPickup, OrderActor.Shop, OrderActor.System),
        new(OrderStatus.InProduction,       OrderStatus.OutForDelivery, OrderActor.Shop, OrderActor.System),
        new(OrderStatus.InProduction,       OrderStatus.ProductionFailed, OrderActor.Shop, OrderActor.System),

        new(OrderStatus.ProductionFailed,   OrderStatus.InProduction,   OrderActor.Shop),   // retry
        new(OrderStatus.ProductionFailed,   OrderStatus.Declined,       OrderActor.Shop),   // give up + refund

        new(OrderStatus.ReadyForPickup,     OrderStatus.Completed,      OrderActor.Customer, OrderActor.Shop),
        new(OrderStatus.OutForDelivery,     OrderStatus.Completed,      OrderActor.Customer, OrderActor.Shop),
    };

    private static readonly HashSet<OrderStatus> Terminal = new()
    {
        OrderStatus.Completed, OrderStatus.Declined, OrderStatus.Cancelled, OrderStatus.Expired
    };

    /// <summary>True if <paramref name="actor"/> may move an order from <paramref name="from"/> to <paramref name="to"/>.</summary>
    public static bool CanTransition(OrderStatus from, OrderStatus to, OrderActor actor)
        => Transitions.Any(t => t.From == from && t.To == to && t.Actors.Contains(actor));

    /// <summary>The states an order in <paramref name="from"/> can move to (ignoring actor) — for diagnostics/UI.</summary>
    public static IReadOnlyList<OrderStatus> AllowedTargets(OrderStatus from)
        => Transitions.Where(t => t.From == from).Select(t => t.To).Distinct().ToList();

    /// <summary>True if the order can no longer transition (a final state).</summary>
    public static bool IsTerminal(OrderStatus status) => Terminal.Contains(status);
}
