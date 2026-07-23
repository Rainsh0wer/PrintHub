using AutoMapper;
using PrintHub.Application.Features.Users.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Users;

public class ProfileMappingProfile : Profile
{
    public ProfileMappingProfile()
    {
        CreateMap<User, ProfileDto>()
            .ForCtorParam(nameof(ProfileDto.Role), o => o.MapFrom(s => s.Role.ToString()));

        CreateMap<User, UserListItemDto>()
            .ForCtorParam(nameof(UserListItemDto.Role), o => o.MapFrom(s => s.Role.ToString()))
            .ForCtorParam(nameof(UserListItemDto.Status), o => o.MapFrom(s => s.Status.ToString()));
    }
}
