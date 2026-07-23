using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Vouchers.Dtos;

public record VoucherAdminDto(
    int Id,
    string Code,
    string? Name,
    string DiscountType,
    decimal DiscountValue,
    decimal MinOrderAmount,
    decimal? MaxDiscountAmount,
    int UsageLimit,
    int UsedCount,
    DateTime ValidFrom,
    DateTime ValidTo,
    bool IsActive,
    int PerUserLimit,
    string? Description);

public record CreateVoucherRequest(
    string Code,
    string? Name,
    VoucherDiscountType DiscountType,
    decimal DiscountValue,
    decimal MinOrderAmount,
    decimal? MaxDiscountAmount,
    int UsageLimit,
    DateTime ValidFrom,
    DateTime ValidTo,
    int PerUserLimit,
    string? Description);

public record UpdateVoucherRequest(
    string? Name,
    decimal DiscountValue,
    decimal MinOrderAmount,
    decimal? MaxDiscountAmount,
    int UsageLimit,
    DateTime ValidFrom,
    DateTime ValidTo,
    bool IsActive,
    int PerUserLimit,
    string? Description);
