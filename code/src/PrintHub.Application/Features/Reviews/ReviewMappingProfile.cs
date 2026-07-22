using AutoMapper;
using PrintHub.Application.Features.Reviews.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Reviews;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<Review, ReviewItemDto>()
            .ForCtorParam(nameof(ReviewItemDto.CustomerName), o => o.MapFrom(s => s.Customer.FullName));
    }
}
