using AutoMapper;
using PrintHub.Application.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Application.Specifications.Shops;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Shops;

/// <summary>
/// Machine and material management (UC-28). Both owner and staff may act
/// (CanOperateShop). Machines/materials are soft-deleted so historical orders
/// keep resolvable references.
/// </summary>
public class ShopResourceService : IShopResourceService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ShopResourceService(IUnitOfWork uow, ICurrentUser currentUser, IMapper mapper)
    {
        _uow = uow;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    // ---- Machines ----

    public async Task<Result<IReadOnlyList<MachineAdminDto>>> ListMachinesAsync(int shopId, CancellationToken ct = default)
    {
        if (!_currentUser.CanOperateShop(shopId))
            return Forbidden<IReadOnlyList<MachineAdminDto>>();

        var machines = await _uow.Repository<Machine>().ListAsync(new MachinesByShopSpecification(shopId), ct);
        return Result.Success(_mapper.Map<IReadOnlyList<MachineAdminDto>>(machines));
    }

    public async Task<Result<MachineAdminDto>> AddMachineAsync(int shopId, AddMachineRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.CanOperateShop(shopId))
            return Forbidden<MachineAdminDto>();

        var machine = new Machine
        {
            ShopId = shopId,
            Name = request.Name.Trim(),
            MachineType = request.MachineType,
            ServiceGroup = request.ServiceGroup,
            Status = MachineStatus.Idle
        };
        await _uow.Repository<Machine>().AddAsync(machine, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(_mapper.Map<MachineAdminDto>(machine));
    }

    public async Task<Result> UpdateMachineStatusAsync(int shopId, int machineId, UpdateMachineStatusRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.CanOperateShop(shopId))
            return Result.Forbidden("You do not have permission to access this shop's data.");

        var machines = _uow.Repository<Machine>();
        var machine = await machines.FirstOrDefaultAsync(new MachineByIdSpecification(machineId), ct);
        if (machine is null || machine.ShopId != shopId)
            return Result.NotFound("This machine could not be found.");

        // BR-75: a machine producing an order cannot be taken offline.
        if (request.Status == MachineStatus.Offline)
        {
            var busy = await _uow.Repository<Order>().AnyAsync(new InProductionOrderByMachineSpecification(machineId), ct);
            if (busy)
                return Result.Conflict("This machine is currently producing an order and cannot be taken offline.");
        }

        machine.Status = request.Status;
        machines.Update(machine);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteMachineAsync(int shopId, int machineId, CancellationToken ct = default)
    {
        if (!_currentUser.CanOperateShop(shopId))
            return Result.Forbidden("You do not have permission to access this shop's data.");

        var machines = _uow.Repository<Machine>();
        var machine = await machines.FirstOrDefaultAsync(new MachineByIdSpecification(machineId), ct);
        if (machine is null || machine.ShopId != shopId)
            return Result.NotFound("This machine could not be found.");

        machine.IsDeleted = true;   // soft delete
        machines.Update(machine);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    // ---- Materials ----

    public async Task<Result<IReadOnlyList<MaterialAdminDto>>> ListMaterialsAsync(int shopId, CancellationToken ct = default)
    {
        if (!_currentUser.CanOperateShop(shopId))
            return Forbidden<IReadOnlyList<MaterialAdminDto>>();

        var materials = await _uow.Repository<Material>().ListAsync(new MaterialsByShopSpecification(shopId), ct);
        return Result.Success(_mapper.Map<IReadOnlyList<MaterialAdminDto>>(materials));
    }

    public async Task<Result<MaterialAdminDto>> AddMaterialAsync(int shopId, AddMaterialRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.CanOperateShop(shopId))
            return Forbidden<MaterialAdminDto>();

        var material = new Material
        {
            ShopId = shopId,
            Name = request.Name.Trim(),
            MaterialType = request.MaterialType,
            Unit = request.Unit,
            StockQuantity = request.StockQuantity,
            LowStockThreshold = request.LowStockThreshold,
            UnitCost = request.UnitCost
        };
        await _uow.Repository<Material>().AddAsync(material, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(_mapper.Map<MaterialAdminDto>(material));
    }

    public async Task<Result> UpdateMaterialStockAsync(int shopId, int materialId, UpdateMaterialStockRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.CanOperateShop(shopId))
            return Result.Forbidden("You do not have permission to access this shop's data.");

        var materials = _uow.Repository<Material>();
        var material = await materials.FirstOrDefaultAsync(new MaterialByIdSpecification(materialId), ct);
        if (material is null || material.ShopId != shopId)
            return Result.NotFound("This material could not be found.");

        material.StockQuantity = request.StockQuantity;
        material.LowStockThreshold = request.LowStockThreshold;
        materials.Update(material);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteMaterialAsync(int shopId, int materialId, CancellationToken ct = default)
    {
        if (!_currentUser.CanOperateShop(shopId))
            return Result.Forbidden("You do not have permission to access this shop's data.");

        var materials = _uow.Repository<Material>();
        var material = await materials.FirstOrDefaultAsync(new MaterialByIdSpecification(materialId), ct);
        if (material is null || material.ShopId != shopId)
            return Result.NotFound("This material could not be found.");

        material.IsDeleted = true;   // soft delete
        materials.Update(material);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static Result<T> Forbidden<T>()
        => Result<T>.Forbidden("You do not have permission to access this shop's data.");
}
