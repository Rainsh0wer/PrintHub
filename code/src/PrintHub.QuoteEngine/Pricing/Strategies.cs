using PrintHub.Contracts.Quoting;
using PrintHub.Domain.Enums;

namespace PrintHub.QuoteEngine.Pricing;

/// <summary>Document services priced per page: unit price × pages × copies.</summary>
public class PerPageStrategy : PricingStrategyBase
{
    public override string PricingModel => Domain.Enums.PricingModel.PerPage.ToString();

    protected override double SizeDriver(EstimateItem item)
        => Math.Max(1, item.PageCount) * Math.Max(1, item.Quantity);

    protected override string Describe(EstimateItem item)
        => $"{item.Quantity} copy(s) × {item.PageCount} page(s)";
}

/// <summary>Finishing/small-format products priced per unit, with quantity tiers.</summary>
public class PerUnitStrategy : PricingStrategyBase
{
    public override string PricingModel => Domain.Enums.PricingModel.PerUnit.ToString();

    protected override double SizeDriver(EstimateItem item)
        => Math.Max(1, item.Quantity);

    protected override string Describe(EstimateItem item)
        => $"{item.Quantity} unit(s)";
}

/// <summary>Fabrication priced by material consumed (with a quality multiplier).</summary>
public class MaterialAndTimeStrategy : PricingStrategyBase
{
    public override string PricingModel => Domain.Enums.PricingModel.MaterialAndTime.ToString();

    protected override double SizeDriver(EstimateItem item)
        => item.EstimatedGrams;

    protected override string Describe(EstimateItem item)
        => $"{item.EstimatedGrams:0.#} g of material";
}
