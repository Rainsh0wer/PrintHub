using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Users.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Users;

/// <summary>Reads and updates the caller's own account. Only self-service — the id
/// always comes from the token, never the request body.</summary>
public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ProfileService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<ProfileDto>> GetMeAsync(int userId, CancellationToken ct = default)
    {
        var user = await _uow.Repository<User>().GetByIdAsync(userId, ct);
        if (user is null)
            return Result<ProfileDto>.NotFound("Account not found.");
        return Result.Success(_mapper.Map<ProfileDto>(user));
    }

    public async Task<Result<ProfileDto>> UpdateMeAsync(int userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Repository<User>().GetByIdAsync(userId, ct);
        if (user is null)
            return Result<ProfileDto>.NotFound("Account not found.");

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.DefaultAddress = request.DefaultAddress;
        user.AvatarUrl = request.AvatarUrl;
        _uow.Repository<User>().Update(user);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(_mapper.Map<ProfileDto>(user));
    }
}
