using AutoMapper;
using PrintHub.Application.Features.Auth.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Auth;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForCtorParam(nameof(UserDto.Role), o => o.MapFrom(s => s.Role.ToString()));
    }
}
