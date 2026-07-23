using AutoMapper;
using PrintHub.Application.Features.Catalog.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Catalog;

public class ServiceTypeMappingProfile : Profile
{
    public ServiceTypeMappingProfile()
    {
        CreateMap<ServiceType, ServiceTypeAdminDto>()
            .ForCtorParam(nameof(ServiceTypeAdminDto.ServiceGroup), o => o.MapFrom(s => s.ServiceGroup.ToString()))
            .ForCtorParam(nameof(ServiceTypeAdminDto.PricingModel), o => o.MapFrom(s => s.PricingModel.ToString()));
    }
}
