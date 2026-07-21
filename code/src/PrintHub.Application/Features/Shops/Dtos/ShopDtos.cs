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
    double? DistanceMeters);

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
    IEnumerable<ShopServiceDto> Services,
    IEnumerable<MachineDto> Machines,
    IEnumerable<ReviewDto> Reviews);

public record ShopServiceDto(
    string ServiceTypeCode,
    string ServiceTypeName,
    string ServiceGroup,
    decimal UnitPrice,
    decimal SetupFee,
    int MinQuantity,
    int LeadTimeMinutes);

public record MachineDto(string Name, string MachineType, string Status);

public record ReviewDto(
    int Rating,
    string? Comment,
    string CustomerName,
    DateTime CreatedAt,
    string? ShopReply);
