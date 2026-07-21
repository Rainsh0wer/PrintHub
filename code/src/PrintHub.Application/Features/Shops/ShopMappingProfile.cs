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
    }
}
