using AutoMapper;
using PrintHub.Application.Features.Vouchers.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Vouchers;

public class VoucherMappingProfile : Profile
{
    public VoucherMappingProfile()
    {
        CreateMap<Voucher, VoucherAdminDto>()
            .ForCtorParam(nameof(VoucherAdminDto.DiscountType), o => o.MapFrom(s => s.DiscountType.ToString()));
    }
}
