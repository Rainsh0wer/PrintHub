using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Orders.Dtos;

/// <summary>One configured line submitted when placing an order (mirrors the quote input).</summary>
public record PlaceOrderItemInput(
    int ServiceTypeId,
    int? DocumentFileId,
    int Quantity,
    int? PageCount,
    string? PaperType,
    ColorMode? ColorMode,
    Sides? Sides,
    string? BindingType,
    string? MaterialName,
    string? QualityProfile,
    decimal? EstimatedGrams,
    string? ItemNote);

/// <summary>UC-15 — place an order from a previously computed quote (paid from the wallet).</summary>
public record PlaceOrderRequest(
    int QuoteId,
    FulfilmentMethod FulfilmentMethod,
    DateTime? PickupSlotStart,
    DateTime? PickupSlotEnd,
    string? DeliveryAddress,
    string? CustomerNote,
    IReadOnlyList<PlaceOrderItemInput> Items,
    string? VoucherCode = null);

/// <summary>Reason carried on cancel/decline transitions.</summary>
public record CancelOrderRequest(string? Reason);

/// <summary>UC-32 — a shop declines an incoming order (reason mandatory; triggers a full refund).</summary>
public record DeclineOrderRequest(DeclineReason Reason, string? Note);

/// <summary>UC-33 — a shop starts production, optionally assigning a specific machine.</summary>
public record StartProductionRequest(int? MachineId);

public record OrderItemDto(
    int Id,
    int ServiceTypeId,
    string? SnapshotFileName,
    int Quantity,
    int? PageCount,
    string? PaperType,
    string? ColorMode,
    string? Sides,
    string? BindingType,
    string? MaterialName,
    string? QualityProfile,
    decimal UnitPrice,
    decimal LineTotal,
    int? EstimatedMinutes,
    string? ItemNote);

public record OrderStatusHistoryDto(
    string? FromStatus,
    string ToStatus,
    string? ActorRole,
    string? Reason,
    DateTime CreatedAt);

/// <summary>Compact order row for lists/queues.</summary>
public record OrderSummaryDto(
    int Id,
    string OrderCode,
    int ShopId,
    string ShopName,
    string Status,
    string FulfilmentMethod,
    decimal TotalAmount,
    int ProgressPercent,
    DateTime? PlacedAt,
    DateTime? EstimatedReadyAt);

/// <summary>Full order view including items and the append-only transition history.</summary>
public record OrderDetailDto(
    int Id,
    string OrderCode,
    int CustomerId,
    int ShopId,
    string ShopName,
    string Status,
    string FulfilmentMethod,
    DateTime? PickupSlotStart,
    DateTime? PickupSlotEnd,
    string? DeliveryAddress,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal TotalAmount,
    decimal? RefundedAmount,
    decimal CommissionAmount,
    int ProgressPercent,
    string? CustomerNote,
    string? CancellationReason,
    DateTime? PlacedAt,
    DateTime? AcceptedAt,
    DateTime? EstimatedReadyAt,
    DateTime? CompletedAt,
    IReadOnlyList<OrderItemDto> Items,
    IReadOnlyList<OrderStatusHistoryDto> History);
