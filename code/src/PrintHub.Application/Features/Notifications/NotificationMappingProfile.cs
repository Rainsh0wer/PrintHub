using AutoMapper;
using PrintHub.Application.Features.Notifications.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Notifications;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationDto>()
            .ForCtorParam(nameof(NotificationDto.Type), o => o.MapFrom(s => s.Type.ToString()));
    }
}
