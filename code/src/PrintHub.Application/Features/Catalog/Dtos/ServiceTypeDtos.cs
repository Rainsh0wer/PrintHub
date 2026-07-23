using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Catalog.Dtos;

public record ServiceTypeAdminDto(
    int Id,
    string Code,
    string Name,
    string ServiceGroup,
    string PricingModel,
    string UnitOfMeasure,
    bool RequiresFile,
    string? Description,
    bool IsActive,
    int DisplayOrder,
    string? IconUrl);

public record CreateServiceTypeRequest(
    string Code,
    string Name,
    ServiceGroup ServiceGroup,
    PricingModel PricingModel,
    string UnitOfMeasure,
    bool RequiresFile,
    string? Description,
    int DisplayOrder,
    string? IconUrl);

public record UpdateServiceTypeRequest(
    string Name,
    string UnitOfMeasure,
    bool RequiresFile,
    string? Description,
    bool IsActive,
    int DisplayOrder,
    string? IconUrl);
