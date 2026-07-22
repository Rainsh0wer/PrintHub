using AutoMapper;
using PrintHub.Application.Features.Orders.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Orders;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderSummaryDto>()
            .ForCtorParam(nameof(OrderSummaryDto.ShopName), o => o.MapFrom(s => s.Shop.Name))
            .ForCtorParam(nameof(OrderSummaryDto.Status), o => o.MapFrom(s => s.Status.ToString()))
            .ForCtorParam(nameof(OrderSummaryDto.FulfilmentMethod), o => o.MapFrom(s => s.FulfilmentMethod.ToString()));

        CreateMap<Order, OrderDetailDto>()
            .ForCtorParam(nameof(OrderDetailDto.ShopName), o => o.MapFrom(s => s.Shop.Name))
            .ForCtorParam(nameof(OrderDetailDto.Status), o => o.MapFrom(s => s.Status.ToString()))
            .ForCtorParam(nameof(OrderDetailDto.FulfilmentMethod), o => o.MapFrom(s => s.FulfilmentMethod.ToString()))
            .ForCtorParam(nameof(OrderDetailDto.History), o => o.MapFrom(s => s.StatusHistory.OrderBy(h => h.CreatedAt)));

        CreateMap<OrderItem, OrderItemDto>()
            .ForCtorParam(nameof(OrderItemDto.ColorMode), o => o.MapFrom(s => s.ColorMode == null ? null : s.ColorMode.ToString()))
            .ForCtorParam(nameof(OrderItemDto.Sides), o => o.MapFrom(s => s.Sides == null ? null : s.Sides.ToString()));

        CreateMap<OrderStatusHistory, OrderStatusHistoryDto>()
            .ForCtorParam(nameof(OrderStatusHistoryDto.FromStatus), o => o.MapFrom(s => s.FromStatus == null ? null : s.FromStatus.ToString()))
            .ForCtorParam(nameof(OrderStatusHistoryDto.ToStatus), o => o.MapFrom(s => s.ToStatus.ToString()))
            .ForCtorParam(nameof(OrderStatusHistoryDto.ActorRole), o => o.MapFrom(s => s.ActorRole == null ? null : s.ActorRole.ToString()));
    }
}
