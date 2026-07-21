namespace PrintHub.Domain.Enums;

/// <summary>
/// The order lifecycle state machine. Transitions and their permitted actors
/// are enforced in the service layer against the transition table; see the SRS
/// "Order Status Definition".
/// </summary>
public enum OrderStatus
{
    Draft = 0,
    Quoted = 1,
    Expired = 2,
    AwaitingAcceptance = 3,
    Accepted = 4,
    InProduction = 5,
    ProductionFailed = 6,
    ReadyForPickup = 7,
    OutForDelivery = 8,
    Completed = 9,
    Declined = 10,
    Cancelled = 11
}

public enum FulfilmentMethod
{
    Pickup = 0,
    Delivery = 1
}

public enum ColorMode
{
    BlackWhite = 0,
    Color = 1
}

public enum Sides
{
    Simplex = 0,
    Duplex = 1
}

/// <summary>
/// Reason a shop records when declining an order. Mandatory on decline.
/// </summary>
public enum DeclineReason
{
    CapacityUnavailable = 0,
    MaterialOutOfStock = 1,
    FileUnreadable = 2,
    SpecificationNotSupported = 3,
    CopyrightConcern = 4
}
