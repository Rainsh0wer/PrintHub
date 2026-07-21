using AutoMapper;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Shops;

public class ShopMappingProfile : Profile
{
    public ShopMappingProfile()
    {
        CreateMap<Shop, ShopSummaryDto>()
            .ForCtorParam(nameof(ShopSummaryDto.ServiceGroups), o => o.MapFrom(s =>
                s.Services.Where(x => x.IsActive)
                          .Select(x => x.ServiceType.ServiceGroup.ToString())
                          .Distinct()))
            .ForCtorParam(nameof(ShopSummaryDto.MinUnitPrice), o => o.MapFrom(s =>
                s.Services.Where(x => x.IsActive).Select(x => (decimal?)x.UnitPrice).Min()))
            .ForCtorParam(nameof(ShopSummaryDto.DistanceMeters), o => o.MapFrom(_ => (double?)null));

        CreateMap<Shop, ShopDetailDto>();

        CreateMap<ShopService, ShopServiceDto>()
            .ForCtorParam(nameof(ShopServiceDto.ServiceTypeCode), o => o.MapFrom(s => s.ServiceType.Code))
            .ForCtorParam(nameof(ShopServiceDto.ServiceTypeName), o => o.MapFrom(s => s.ServiceType.Name))
            .ForCtorParam(nameof(ShopServiceDto.ServiceGroup), o => o.MapFrom(s => s.ServiceType.ServiceGroup.ToString()));

        CreateMap<Machine, MachineDto>()
            .ForCtorParam(nameof(MachineDto.MachineType), o => o.MapFrom(s => s.MachineType.ToString()))
            .ForCtorParam(nameof(MachineDto.Status), o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<Review, ReviewDto>()
            .ForCtorParam(nameof(ReviewDto.CustomerName), o => o.MapFrom(s => s.Customer.FullName));

        CreateMap<Shop, ShopApplicationDto>()
            .ForCtorParam(nameof(ShopApplicationDto.Status), o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<Shop, ShopAdminListItemDto>()
            .ForCtorParam(nameof(ShopAdminListItemDto.OwnerName), o => o.MapFrom(s => s.Owner.FullName))
            .ForCtorParam(nameof(ShopAdminListItemDto.OwnerEmail), o => o.MapFrom(s => s.Owner.Email))
            .ForCtorParam(nameof(ShopAdminListItemDto.Status), o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<ShopService, RateCardEntryDto>()
            .ForCtorParam(nameof(RateCardEntryDto.ServiceTypeCode), o => o.MapFrom(s => s.ServiceType.Code))
            .ForCtorParam(nameof(RateCardEntryDto.ServiceTypeName), o => o.MapFrom(s => s.ServiceType.Name))
            .ForCtorParam(nameof(RateCardEntryDto.ServiceGroup), o => o.MapFrom(s => s.ServiceType.ServiceGroup.ToString()))
            .ForCtorParam(nameof(RateCardEntryDto.PricingModel), o => o.MapFrom(s => s.ServiceType.PricingModel.ToString()));

        CreateMap<PriceRule, PriceRuleDto>()
            .ForCtorParam(nameof(PriceRuleDto.RuleType), o => o.MapFrom(s => s.RuleType.ToString()));

        CreateMap<ShopStaff, StaffDto>()
            .ForCtorParam(nameof(StaffDto.Name), o => o.MapFrom(s => s.User.FullName))
            .ForCtorParam(nameof(StaffDto.Email), o => o.MapFrom(s => s.User.Email));

        CreateMap<Machine, MachineAdminDto>()
            .ForCtorParam(nameof(MachineAdminDto.MachineType), o => o.MapFrom(s => s.MachineType.ToString()))
            .ForCtorParam(nameof(MachineAdminDto.ServiceGroup), o => o.MapFrom(s => s.ServiceGroup.ToString()))
            .ForCtorParam(nameof(MachineAdminDto.Status), o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<Material, MaterialAdminDto>()
            .ForCtorParam(nameof(MaterialAdminDto.MaterialType), o => o.MapFrom(s => s.MaterialType.ToString()));

        // OData projections — kept translatable (no enum.ToString, no in-memory calls)
        // so AutoMapper ProjectTo produces a query EF pushes to SQL.
        CreateMap<Shop, ShopODataDto>();
        CreateMap<ShopService, ShopServiceODataDto>()
            .ForMember(d => d.ServiceTypeName, o => o.MapFrom(s => s.ServiceType.Name))
            .ForMember(d => d.ServiceGroup, o => o.MapFrom(s => s.ServiceType.ServiceGroup));
    }
}
