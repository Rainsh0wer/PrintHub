using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Vouchers.Dtos;

namespace PrintHub.Application.Features.Vouchers;

public interface IVoucherAdminService
{
    Task<Result<IReadOnlyList<VoucherAdminDto>>> ListAsync(CancellationToken ct = default);
    Task<Result<VoucherAdminDto>> CreateAsync(CreateVoucherRequest request, CancellationToken ct = default);
    Task<Result<VoucherAdminDto>> UpdateAsync(int id, UpdateVoucherRequest request, CancellationToken ct = default);
    Task<Result> DeactivateAsync(int id, CancellationToken ct = default);
}
