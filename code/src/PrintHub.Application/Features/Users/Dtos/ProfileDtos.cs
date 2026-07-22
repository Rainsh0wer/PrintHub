namespace PrintHub.Application.Features.Users.Dtos;

/// <summary>UC-06 — the caller's own account profile.</summary>
public record ProfileDto(
    int Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    string Role,
    decimal WalletBalance,
    string? DefaultAddress,
    string? AvatarUrl,
    DateTime CreatedAt,
    DateTime? LastLoginAt);

/// <summary>UC-07 — update display name, phone, default address, avatar.</summary>
public record UpdateProfileRequest(
    string FullName,
    string? PhoneNumber,
    string? DefaultAddress,
    string? AvatarUrl);
