using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Vouchers.Dtos;
using PrintHub.Application.Specifications.Orders;
using PrintHub.Application.Specifications.Vouchers;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Vouchers;

public class VoucherService : IVoucherService
{
    private readonly IUnitOfWork _uow;

    public VoucherService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<VoucherApplicationDto>> ApplyToQuoteAsync(int customerId, int quoteId, string code, CancellationToken ct = default)
    {
        var quote = await _uow.Repository<Quote>().GetByIdAsync(quoteId, ct);
        if (quote is null || quote.CustomerId != customerId)
            return Result<VoucherApplicationDto>.NotFound("Quote not found.");

        var applied = await ValidateForOrderAsync(customerId, code, quote.SubTotal, ct);
        if (applied.IsFailure)
            return Result<VoucherApplicationDto>.Fail(applied.Error!, applied.ErrorType);

        var discount = applied.Value!.Discount;
        return Result.Success(new VoucherApplicationDto(quoteId, code, quote.SubTotal, discount, quote.SubTotal - discount));
    }

    public async Task<Result<AppliedVoucher>> ValidateForOrderAsync(int customerId, string code, decimal subtotal, CancellationToken ct = default)
    {
        var voucher = await _uow.Repository<Voucher>().FirstOrDefaultAsync(new VoucherByCodeSpecification(code), ct);
        if (voucher is null)
            return Result<AppliedVoucher>.NotFound("Voucher code not found.");

        var now = DateTime.UtcNow;
        if (!voucher.IsActive)
            return Result<AppliedVoucher>.Conflict("This voucher is not active.");
        if (now < voucher.ValidFrom || now > voucher.ValidTo)
            return Result<AppliedVoucher>.Conflict("This voucher is not valid at this time.");
        if (voucher.UsageLimit > 0 && voucher.UsedCount >= voucher.UsageLimit)
            return Result<AppliedVoucher>.Conflict("This voucher has reached its usage limit.");
        if (subtotal < voucher.MinOrderAmount)
            return Result<AppliedVoucher>.Conflict($"This voucher requires a minimum order of {voucher.MinOrderAmount:0} VND.");

        if (voucher.PerUserLimit > 0)
        {
            var used = await _uow.Repository<Order>().CountAsync(new OrdersByCustomerAndVoucherCountSpecification(customerId, voucher.Id), ct);
            if (used >= voucher.PerUserLimit)
                return Result<AppliedVoucher>.Conflict("You have already used this voucher.");
        }

        return Result.Success(new AppliedVoucher(voucher.Id, ComputeDiscount(voucher, subtotal)));
    }

    private static decimal ComputeDiscount(Voucher voucher, decimal subtotal)
    {
        var discount = voucher.DiscountType == VoucherDiscountType.Percent
            ? subtotal * voucher.DiscountValue / 100m
            : voucher.DiscountValue;

        if (voucher.MaxDiscountAmount is { } cap && discount > cap)
            discount = cap;

        return Math.Min(discount, subtotal);
    }
}
