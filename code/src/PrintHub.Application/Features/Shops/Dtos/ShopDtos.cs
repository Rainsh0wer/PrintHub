namespace PrintHub.Application.Features.Shops.Dtos;

/// <summary>Row in the shop directory / search results.</summary>
public record ShopSummaryDto(
    int Id,
    string Name,
    string District,
    string City,
    double RatingAverage,
    int RatingCount,
    IEnumerable<string> ServiceGroups,
    decimal? MinUnitPrice,
    decimal? MaxUnitPrice,
    double? DistanceMeters,
    string? LogoUrl,
    string? CoverImageUrl);

/// <summary>Full shop profile shown on the detail page (UC-10).</summary>
public record ShopDetailDto(
    int Id,
    string Name,
    string? Description,
    string AddressLine,
    string District,
    string City,
    string? PhoneNumber,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    double RatingAverage,
    int RatingCount,
    string? LogoUrl,
    string? CoverImageUrl,
    IEnumerable<ShopServiceDto> Services,
    IEnumerable<MachineDto> Machines,
    IEnumerable<ReviewDto> Reviews,
    IEnumerable<ShopGalleryImageDto> Gallery);

public record ShopGalleryImageDto(string Url, string? Caption);

public record ShopServiceDto(
    string ServiceTypeCode,
    string ServiceTypeName,
    string ServiceGroup,
    decimal UnitPrice,
    decimal SetupFee,
    int MinQuantity,
    int LeadTimeMinutes,
    string? IconUrl);

public record MachineDto(string Name, string MachineType, string Status, string? PhotoUrl);

public record ReviewDto(
    int Rating,
    string? Comment,
    string CustomerName,
    string? CustomerAvatarUrl,
    DateTime CreatedAt,
    string? ShopReply,
    string? PhotoUrls);
