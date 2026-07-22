using AutoMapper;
using PrintHub.Application.Features.Wallet.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Wallet;

public class WalletMappingProfile : Profile
{
    public WalletMappingProfile()
    {
        CreateMap<WalletTransaction, WalletTransactionDto>()
            .ForCtorParam(nameof(WalletTransactionDto.Type), o => o.MapFrom(s => s.Type.ToString()))
            .ForCtorParam(nameof(WalletTransactionDto.Status), o => o.MapFrom(s => s.Status.ToString()));
    }
}
