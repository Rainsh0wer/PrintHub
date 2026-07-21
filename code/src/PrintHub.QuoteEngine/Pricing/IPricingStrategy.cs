using PrintHub.Contracts.Quoting;

namespace PrintHub.QuoteEngine.Pricing;

/// <summary>
/// Strategy pattern: one pricing algorithm per pricing model. The engine selects
/// the strategy by the item's pricing model, so a new model is a new strategy
/// rather than a change to the engine.
/// </summary>
public interface IPricingStrategy
{
    /// <summary>The pricing model this strategy handles (matches the Domain PricingModel name).</summary>
    string PricingModel { get; }

    LineBreakdown Calculate(EstimateItem item);
}
