namespace PrintHub.Application.Features.Shops.Dtos;

/// <summary>Application to open a shop (UC-25).</summary>
public record ShopApplicationRequest(
    string Name,
    string? Description,
    string AddressLine,
    string District,
    string City,
    string? PhoneNumber,
    TimeOnly OpenTime,
    TimeOnly CloseTime);

/// <summary>Owner's view of one of their shops and its onboarding status.</summary>
public record ShopApplicationDto(
    int Id,
    string Name,
    string Status,
    string District,
    string City,
    string? ReviewNote,
    DateTime CreatedAt);

/// <summary>Admin's list-item view of a shop (application review / management).</summary>
public record ShopAdminListItemDto(
    int Id,
    string Name,
    string OwnerName,
    string OwnerEmail,
    string District,
    string City,
    string Status,
    double RatingAverage,
    DateTime CreatedAt);

/// <summary>Body for actions that require a mandatory reason (BR-94).</summary>
public record ReasonRequest(string Reason);
