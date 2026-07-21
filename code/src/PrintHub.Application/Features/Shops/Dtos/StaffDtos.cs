namespace PrintHub.Application.Features.Shops.Dtos;

/// <summary>A staff membership at a shop (UC-29).</summary>
public record StaffDto(
    int Id,
    int UserId,
    string Name,
    string Email,
    string? Position,
    DateTime JoinedAt,
    bool IsActive);

/// <summary>Grant a user operational access to a shop.</summary>
public record GrantStaffRequest(string Email, string? Position);
