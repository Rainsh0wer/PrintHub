namespace PrintHub.Application.Features.Platform.Dtos;

public record CommissionDto(decimal CommissionRate, DateTime UpdatedAt);

public record SetCommissionRequest(decimal Rate);

/// <summary>BR-47 — the share of the total a shop keeps when an accepted order is cancelled.</summary>
public record CancellationFeeDto(decimal CancellationFeeRate, DateTime UpdatedAt);

public record SetCancellationFeeRequest(decimal Rate);
