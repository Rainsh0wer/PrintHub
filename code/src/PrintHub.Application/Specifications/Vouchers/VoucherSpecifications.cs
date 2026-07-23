using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.Vouchers;

public sealed class AllVouchersSpecification : BaseSpecification<Voucher>
{
    public AllVouchersSpecification()
    {
        ApplyOrderByDescending(v => v.Id);
    }
}

public sealed class VoucherByCodeSpecification : BaseSpecification<Voucher>
{
    public VoucherByCodeSpecification(string code)
        : base(v => v.Code == code)
    {
    }
}
