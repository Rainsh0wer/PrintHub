using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Auth.Dtos;
using PrintHub.Application.Specifications.RefreshTokens;
using PrintHub.Application.Specifications.Shops;
using PrintHub.Application.Specifications.Users;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Auth;

/// <summary>
/// Implements the authentication use cases. All business rules (email
/// uniqueness, generic login errors, lock check, token rotation, revoke-all on
/// password change) live here, not in the controller.
/// </summary>
public class AuthService : IAuthService
{
    private const string ResetMarker = "pwd-reset";
    private static readonly TimeSpan ResetValidity = TimeSpan.FromMinutes(30);

    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwt;
    private readonly IMapper _mapper;

    public AuthService(IUnitOfWork uow, IPasswordHasher passwordHasher, IJwtTokenService jwt, IMapper mapper)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
        _mapper = mapper;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var users = _uow.Repository<User>();

        // BR-1: one account per email.
        if (await users.AnyAsync(new UserByEmailSpecification(request.Email), ct))
            return Result<AuthResponse>.Conflict("This email address is already registered. Please log in or reset your password.");

        // BR-2 / BR-3: new accounts are Customers with a zero balance and a BCrypt hash.
        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRole.Customer,
            WalletBalance = 0
        };

        await users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var response = await IssueTokensAsync(user, Array.Empty<int>(), ct);
        return Result<AuthResponse>.Ok(response);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var users = _uow.Repository<User>();
        var user = await users.FirstOrDefaultAsync(new UserByEmailSpecification(request.Email), ct);

        // BR-5: a single generic message so account existence cannot be probed.
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Fail("Incorrect email or password.", ErrorType.Unauthorized);

        // BR-6: locked accounts are not issued a token.
        if (user.Status == UserStatus.Locked)
            return Result<AuthResponse>.Fail("This account has been locked. Please contact support.", ErrorType.Forbidden);

        var shopIds = await GetShopIdsAsync(user, ct);
        var response = await IssueTokensAsync(user, shopIds, ct);
        return Result<AuthResponse>.Ok(response);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request, CancellationToken ct = default)
    {
        var tokens = _uow.Repository<RefreshToken>();
        var existing = await tokens.FirstOrDefaultAsync(new RefreshTokenByValueSpecification(request.RefreshToken), ct);

        if (existing is null || !existing.IsActive)
            return Result<AuthResponse>.Fail("Invalid or expired refresh token.", ErrorType.Unauthorized);

        // Rotate: the presented token is revoked and a fresh pair is issued.
        existing.RevokedAt = DateTime.UtcNow;
        tokens.Update(existing);

        var shopIds = await GetShopIdsAsync(existing.User, ct);
        var response = await IssueTokensAsync(existing.User, shopIds, ct);
        return Result<AuthResponse>.Ok(response);
    }

    public async Task<Result> LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        var tokens = _uow.Repository<RefreshToken>();
        var existing = await tokens.FirstOrDefaultAsync(new RefreshTokenByValueSpecification(refreshToken), ct);

        // BR-10: revoke server-side. Idempotent if already revoked or unknown.
        if (existing is not null && existing.RevokedAt is null)
        {
            existing.RevokedAt = DateTime.UtcNow;
            tokens.Update(existing);
            await _uow.SaveChangesAsync(ct);
        }

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var users = _uow.Repository<User>();
        var user = await users.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.NotFound("Account not found.");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            return Result.Failure("The current password is incorrect.");

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        users.Update(user);

        // BR-9: changing the password revokes every active refresh token.
        var tokens = _uow.Repository<RefreshToken>();
        var active = await tokens.ListAsync(new ActiveRefreshTokensByUserSpecification(userId), ct);
        foreach (var token in active)
        {
            token.RevokedAt = DateTime.UtcNow;
            tokens.Update(token);
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<ForgotPasswordResponse>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Repository<User>().FirstOrDefaultAsync(new UserByEmailSpecification(request.Email.Trim()), ct);
        const string message = "If that email is registered, a password reset link has been generated.";
        if (user is null)
            return Result.Success(new ForgotPasswordResponse(message, null));

        var token = Guid.NewGuid().ToString("N");
        await _uow.Repository<RefreshToken>().AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = token,
            CreatedByIp = ResetMarker,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(ResetValidity)
        }, ct);
        await _uow.SaveChangesAsync(ct);

        // Dev build returns the token directly; a real deployment emails it.
        return Result.Success(new ForgotPasswordResponse(message, token));
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        var users = _uow.Repository<User>();
        var tokens = _uow.Repository<RefreshToken>();
        var user = await users.FirstOrDefaultAsync(new UserByEmailSpecification(request.Email.Trim()), ct);
        var reset = await tokens.FirstOrDefaultAsync(new RefreshTokenByValueSpecification(request.Token), ct);

        if (user is null || reset is null || reset.UserId != user.Id
            || reset.CreatedByIp != ResetMarker || reset.RevokedAt is not null || reset.ExpiresAt <= DateTime.UtcNow)
            return Result.Failure("This reset link is invalid or has expired.");

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        users.Update(user);

        var active = await tokens.ListAsync(new ActiveRefreshTokensByUserSpecification(user.Id), ct);
        foreach (var t in active)
        {
            t.RevokedAt = DateTime.UtcNow;
            tokens.Update(t);
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    /// <summary>Issues a token pair, persists the refresh token, and builds the response.</summary>
    private async Task<AuthResponse> IssueTokensAsync(User user, IReadOnlyCollection<int> shopIds, CancellationToken ct)
    {
        var tokenResult = _jwt.CreateTokens(user, shopIds);

        await _uow.Repository<RefreshToken>().AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = tokenResult.RefreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = tokenResult.RefreshTokenExpiresAt
        }, ct);
        await _uow.SaveChangesAsync(ct);

        var userDto = _mapper.Map<UserDto>(user);
        return new AuthResponse(
            tokenResult.AccessToken,
            tokenResult.AccessTokenExpiresAt,
            tokenResult.RefreshToken,
            tokenResult.RefreshTokenExpiresAt,
            userDto);
    }

    /// <summary>
    /// Shop-membership ids for the access token — the union of shops the user owns
    /// and shops they are active staff of. This handles the real case where a user
    /// both owns a shop and works at another.
    /// </summary>
    private async Task<IReadOnlyCollection<int>> GetShopIdsAsync(User user, CancellationToken ct)
    {
        if (user.Role == UserRole.Admin)
            return Array.Empty<int>();

        var owned = await _uow.Repository<Shop>().ListAsync(new ShopsByOwnerSpecification(user.Id), ct);
        var staff = await _uow.Repository<ShopStaff>().ListAsync(new ActiveStaffByUserSpecification(user.Id), ct);

        return owned.Select(s => s.Id)
            .Concat(staff.Select(s => s.ShopId))
            .Distinct()
            .ToArray();
    }
}
