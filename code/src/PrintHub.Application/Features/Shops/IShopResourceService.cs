using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Shops;

/// <summary>
/// Machine registry and material stock management (UC-28). Operational, so both
/// the owner and shop staff may act (CanOperateShop).
/// </summary>
public interface IShopResourceService
{
    Task<Result<IReadOnlyList<MachineAdminDto>>> ListMachinesAsync(int shopId, CancellationToken ct = default);
    Task<Result<MachineAdminDto>> AddMachineAsync(int shopId, AddMachineRequest request, CancellationToken ct = default);
    Task<Result> UpdateMachineStatusAsync(int shopId, int machineId, UpdateMachineStatusRequest request, CancellationToken ct = default);
    Task<Result> DeleteMachineAsync(int shopId, int machineId, CancellationToken ct = default);

    Task<Result<IReadOnlyList<MaterialAdminDto>>> ListMaterialsAsync(int shopId, CancellationToken ct = default);
    Task<Result<MaterialAdminDto>> AddMaterialAsync(int shopId, AddMaterialRequest request, CancellationToken ct = default);
    Task<Result> UpdateMaterialStockAsync(int shopId, int materialId, UpdateMaterialStockRequest request, CancellationToken ct = default);
    Task<Result> DeleteMaterialAsync(int shopId, int materialId, CancellationToken ct = default);
}
