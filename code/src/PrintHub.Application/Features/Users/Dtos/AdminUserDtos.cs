namespace PrintHub.Application.Features.Users.Dtos;

public record UserListItemDto(
    int Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    string Role,
    string Status,
    decimal WalletBalance,
    DateTime CreatedAt,
    DateTime? LastLoginAt);
