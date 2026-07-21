using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Specifications.Shops;

public sealed class MachinesByShopSpecification : BaseSpecification<Machine>
{
    public MachinesByShopSpecification(int shopId)
        : base(m => m.ShopId == shopId)
    {
        ApplyOrderBy(m => m.Name);
    }
}

public sealed class MachineByIdSpecification : BaseSpecification<Machine>
{
    public MachineByIdSpecification(int machineId)
        : base(m => m.Id == machineId)
    {
    }
}

public sealed class MaterialsByShopSpecification : BaseSpecification<Material>
{
    public MaterialsByShopSpecification(int shopId)
        : base(m => m.ShopId == shopId)
    {
        ApplyOrderBy(m => m.Name);
    }
}

public sealed class MaterialByIdSpecification : BaseSpecification<Material>
{
    public MaterialByIdSpecification(int materialId)
        : base(m => m.Id == materialId)
    {
    }
}

/// <summary>Any in-production order assigned to a machine — blocks taking it offline (BR-75).</summary>
public sealed class InProductionOrderByMachineSpecification : BaseSpecification<Order>
{
    public InProductionOrderByMachineSpecification(int machineId)
        : base(o => o.MachineId == machineId && o.Status == OrderStatus.InProduction)
    {
    }
}
