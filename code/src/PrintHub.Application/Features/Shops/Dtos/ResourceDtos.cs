using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Shops.Dtos;

public record MachineAdminDto(
    int Id,
    string Name,
    string MachineType,
    string ServiceGroup,
    string Status);

public record AddMachineRequest(string Name, MachineType MachineType, ServiceGroup ServiceGroup);

public record UpdateMachineStatusRequest(MachineStatus Status);

public record MaterialAdminDto(
    int Id,
    string Name,
    string MaterialType,
    string Unit,
    decimal StockQuantity,
    decimal LowStockThreshold,
    bool IsLowStock);

public record AddMaterialRequest(
    string Name,
    MaterialType MaterialType,
    string Unit,
    decimal StockQuantity,
    decimal LowStockThreshold,
    decimal UnitCost);

public record UpdateMaterialStockRequest(decimal StockQuantity, decimal LowStockThreshold);
