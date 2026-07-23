using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Vouchers.Dtos;

namespace PrintHub.Application.Features.Vouchers;

/// <summary>Customer-facing voucher use: preview a discount on a quote (UC-14) and
/// validate a code at order placement.</summary>
public interface IVoucherService
{
    /// <summary>UC-14 — apply a voucher to a quote and return the recomputed total (preview only).</summary>
    Task<Result<VoucherApplicationDto>> ApplyToQuoteAsync(int customerId, int quoteId, string code, CancellationToken ct = default);

    /// <summary>Validate a code against a subtotal for a customer; returns the voucher id + discount.</summary>
    Task<Result<AppliedVoucher>> ValidateForOrderAsync(int customerId, string code, decimal subtotal, CancellationToken ct = default);
}
