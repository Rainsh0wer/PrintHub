using PrintHub.Contracts.Quoting;

namespace PrintHub.QuoteEngine.Pricing;

/// <summary>
/// Shared pricing mechanics. The effective unit price is the base price adjusted
/// by every applicable rule (option multipliers/surcharges, or the matching
/// quantity tier). Concrete strategies differ only in the "size driver" — what
/// the unit price multiplies by — and the human-readable description.
/// </summary>
public abstract class PricingStrategyBase : IPricingStrategy
{
    public abstract string PricingModel { get; }

    protected abstract double SizeDriver(EstimateItem item);
    protected abstract string Describe(EstimateItem item);

    public LineBreakdown Calculate(EstimateItem item)
    {
        var applied = new List<string>();
        var multiplier = 1.0;
        var flatExtra = 0.0;

        foreach (var rule in item.Rules)
        {
            if (!RuleApplies(rule, item)) continue;
            if (rule.Multiplier > 0) multiplier *= rule.Multiplier;
            flatExtra += rule.FlatExtra;
            applied.Add($"{rule.RuleType}:{rule.OptionKey}");
        }

        var effectiveUnit = item.UnitPrice * multiplier + flatExtra;
        var size = Math.Max(0, SizeDriver(item));
        var lineTotal = effectiveUnit * size + item.SetupFee;
        var minutes = (int)Math.Ceiling(item.LeadTimeMinutes * size);

        var line = new LineBreakdown
        {
            Description = Describe(item),
            EffectiveUnitPrice = Math.Round(effectiveUnit, 2),
            LineTotal = Math.Round(lineTotal, 2),
            Minutes = minutes
        };
        line.AppliedRules.AddRange(applied);
        return line;
    }

    private static bool RuleApplies(PricingRuleInput rule, EstimateItem item)
    {
        if (rule.RuleType == "QuantityTier")
        {
            var low = rule.HasMin ? rule.MinQuantity : int.MinValue;
            var high = rule.HasMax ? rule.MaxQuantity : int.MaxValue;
            return item.Quantity >= low && item.Quantity <= high;
        }

        return item.SelectedOptions.Contains(rule.OptionKey);
    }
}
