using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Complaints.Dtos;

/// <summary>UC-24 — a customer raises a dispute against a completed order.</summary>
public record RaiseComplaintRequest(
    int OrderId,
    ComplaintReason Reason,
    string Description,
    string? AttachmentUrls);

/// <summary>UC-35 — a shop proposes a resolution (reprint or refund) to an open complaint.</summary>
public record RespondComplaintRequest(
    ComplaintResolution ProposedResolution,
    decimal? RefundAmount,
    string? ShopResponse);

/// <summary>UC-41 — an administrator's final ruling on an escalated complaint.</summary>
public record AdjudicateComplaintRequest(
    bool UpholdRefund,
    decimal? RefundAmount,
    string? AdminRuling);

public record ComplaintDto(
    int Id,
    int OrderId,
    string OrderCode,
    int CustomerId,
    int ShopId,
    string ShopName,
    string Reason,
    string Description,
    string Status,
    string? ProposedResolution,
    string? ShopResponse,
    string? AdminRuling,
    decimal? RefundAmount,
    int? ReplacementOrderId,
    DateTime CreatedAt,
    DateTime? RespondedAt,
    DateTime? EscalatedAt,
    DateTime? ResolvedAt,
    DateTime? ClosedAt);
