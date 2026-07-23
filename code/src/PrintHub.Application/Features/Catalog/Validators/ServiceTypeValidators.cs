using FluentValidation;
using PrintHub.Application.Features.Catalog.Dtos;

namespace PrintHub.Application.Features.Catalog.Validators;

public class CreateServiceTypeRequestValidator : AbstractValidator<CreateServiceTypeRequest>
{
    public CreateServiceTypeRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(40).Matches("^[A-Z0-9_]+$")
            .WithMessage("Code must be uppercase letters, digits, or underscores.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.UnitOfMeasure).NotEmpty().MaximumLength(20);
    }
}

public class UpdateServiceTypeRequestValidator : AbstractValidator<UpdateServiceTypeRequest>
{
    public UpdateServiceTypeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.UnitOfMeasure).NotEmpty().MaximumLength(20);
    }
}
