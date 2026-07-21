namespace PrintHub.Domain.Enums;

public enum ComplaintReason
{
    WrongOutput = 0,
    MissingPages = 1,
    QualityDefect = 2,
    Late = 3,
    Other = 4
}

public enum ComplaintStatus
{
    Open = 0,
    ShopResponded = 1,
    Resolved = 2,
    Escalated = 3,
    Closed = 4
}

public enum ComplaintResolution
{
    Reprint = 0,
    Refund = 1,
    Rejected = 2
}
