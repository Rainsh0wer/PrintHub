namespace PrintHub.Domain.Enums;

public enum ShopStatus
{
    Draft = 0,
    PendingReview = 1,
    Active = 2,
    Rejected = 3,
    Suspended = 4
}

/// <summary>
/// The three service groups, each with a distinct pricing model. This is the
/// axis that makes the Strategy pattern for pricing meaningful.
/// </summary>
public enum ServiceGroup
{
    Document = 0,
    Finishing = 1,
    Fabrication = 2
}

/// <summary>
/// Selects the pricing strategy applied by the Quote Engine for a service type.
/// </summary>
public enum PricingModel
{
    PerPage = 0,
    PerUnit = 1,
    MaterialAndTime = 2
}

public enum PriceRuleType
{
    PaperType = 0,
    ColorMode = 1,
    Sides = 2,
    BindingType = 3,
    Material = 4,
    QualityProfile = 5,
    QuantityTier = 6
}

public enum MachineType
{
    Printer = 0,
    Plotter = 1,
    Printer3D = 2,
    LaserCutter = 3,
    Finishing = 4
}

public enum MachineStatus
{
    Idle = 0,
    Busy = 1,
    Maintenance = 2,
    Offline = 3
}

public enum MaterialType
{
    Paper = 0,
    Filament = 1,
    Sheet = 2,
    Consumable = 3
}
