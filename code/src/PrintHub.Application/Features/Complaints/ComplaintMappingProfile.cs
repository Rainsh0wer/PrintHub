using AutoMapper;
using PrintHub.Application.Features.Complaints.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Complaints;

public class ComplaintMappingProfile : Profile
{
    public ComplaintMappingProfile()
    {
        CreateMap<Complaint, ComplaintDto>()
            .ForCtorParam(nameof(ComplaintDto.OrderCode), o => o.MapFrom(s => s.Order.OrderCode))
            .ForCtorParam(nameof(ComplaintDto.ShopName), o => o.MapFrom(s => s.Shop.Name))
            .ForCtorParam(nameof(ComplaintDto.Reason), o => o.MapFrom(s => s.Reason.ToString()))
            .ForCtorParam(nameof(ComplaintDto.Status), o => o.MapFrom(s => s.Status.ToString()))
            .ForCtorParam(nameof(ComplaintDto.ProposedResolution), o => o.MapFrom(s => s.ProposedResolution == null ? null : s.ProposedResolution.ToString()));
    }
}
