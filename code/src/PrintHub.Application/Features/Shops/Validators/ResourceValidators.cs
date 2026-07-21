using FluentValidation;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Shops.Validators;

public class AddMachineRequestValidator : AbstractValidator<AddMachineRequest>
{
    public AddMachineRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class AddMaterialRequestValidator : AbstractValidator<AddMaterialRequest>
{
    public AddMaterialRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(20);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);       // BR-76
        RuleFor(x => x.LowStockThreshold).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0);
    }
}

public class UpdateMaterialStockRequestValidator : AbstractValidator<UpdateMaterialStockRequest>
{
    public UpdateMaterialStockRequestValidator()
    {
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);       // BR-76
        RuleFor(x => x.LowStockThreshold).GreaterThanOrEqualTo(0);
    }
}
