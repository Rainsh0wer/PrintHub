namespace PrintHub.Application.Features.Vouchers.Dtos;

public record ApplyVoucherRequest(string Code);

public record VoucherApplicationDto(
    int QuoteId,
    string VoucherCode,
    decimal SubTotal,
    decimal Discount,
    decimal Total);

/// <summary>Result of validating a voucher against a subtotal, used at order placement.</summary>
public record AppliedVoucher(int VoucherId, decimal Discount);
