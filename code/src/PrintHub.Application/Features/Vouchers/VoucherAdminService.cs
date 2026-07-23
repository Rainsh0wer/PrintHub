using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Vouchers.Dtos;
using PrintHub.Application.Specifications.Vouchers;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Vouchers;

public class VoucherAdminService : IVoucherAdminService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public VoucherAdminService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<VoucherAdminDto>>> ListAsync(CancellationToken ct = default)
    {
        var vouchers = await _uow.Repository<Voucher>().ListAsync(new AllVouchersSpecification(), ct);
        return Result.Success(_mapper.Map<IReadOnlyList<VoucherAdminDto>>(vouchers));
    }

    public async Task<Result<VoucherAdminDto>> CreateAsync(CreateVoucherRequest request, CancellationToken ct = default)
    {
        if (request.ValidTo <= request.ValidFrom)
            return Result<VoucherAdminDto>.Fail("The valid-to date must be after the valid-from date.");

        var repo = _uow.Repository<Voucher>();
        if (await repo.AnyAsync(new VoucherByCodeSpecification(request.Code), ct))
            return Result<VoucherAdminDto>.Conflict("A voucher with this code already exists.");

        var voucher = new Voucher
        {
            Code = request.Code,
            Name = request.Name,
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            MinOrderAmount = request.MinOrderAmount,
            MaxDiscountAmount = request.MaxDiscountAmount,
            UsageLimit = request.UsageLimit,
            UsedCount = 0,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            PerUserLimit = request.PerUserLimit,
            Description = request.Description,
            IsActive = true
        };
        await repo.AddAsync(voucher, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success(_mapper.Map<VoucherAdminDto>(voucher));
    }

    public async Task<Result<VoucherAdminDto>> UpdateAsync(int id, UpdateVoucherRequest request, CancellationToken ct = default)
    {
        if (request.ValidTo <= request.ValidFrom)
            return Result<VoucherAdminDto>.Fail("The valid-to date must be after the valid-from date.");

        var repo = _uow.Repository<Voucher>();
        var voucher = await repo.GetByIdAsync(id, ct);
        if (voucher is null)
            return Result<VoucherAdminDto>.NotFound("Voucher not found.");

        voucher.Name = request.Name;
        voucher.DiscountValue = request.DiscountValue;
        voucher.MinOrderAmount = request.MinOrderAmount;
        voucher.MaxDiscountAmount = request.MaxDiscountAmount;
        voucher.UsageLimit = request.UsageLimit;
        voucher.ValidFrom = request.ValidFrom;
        voucher.ValidTo = request.ValidTo;
        voucher.IsActive = request.IsActive;
        voucher.PerUserLimit = request.PerUserLimit;
        voucher.Description = request.Description;
        repo.Update(voucher);
        await _uow.SaveChangesAsync(ct);
        return Result.Success(_mapper.Map<VoucherAdminDto>(voucher));
    }

    public async Task<Result> DeactivateAsync(int id, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Voucher>();
        var voucher = await repo.GetByIdAsync(id, ct);
        if (voucher is null)
            return Result.NotFound("Voucher not found.");

        voucher.IsActive = false;
        repo.Update(voucher);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
