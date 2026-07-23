namespace PrintHub.Application.Features.Platform.Dtos;

public record CommissionDto(decimal CommissionRate, DateTime UpdatedAt);

public record SetCommissionRequest(decimal Rate);
