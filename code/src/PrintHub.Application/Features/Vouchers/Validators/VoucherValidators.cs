using FluentValidation;
using PrintHub.Application.Features.Vouchers.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Vouchers.Validators;

public class CreateVoucherRequestValidator : AbstractValidator<CreateVoucherRequest>
{
    public CreateVoucherRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.DiscountValue).LessThanOrEqualTo(100)
            .When(x => x.DiscountType == VoucherDiscountType.Percent)
            .WithMessage("A percentage discount cannot exceed 100.");
        RuleFor(x => x.MinOrderAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UsageLimit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PerUserLimit).GreaterThanOrEqualTo(0);
    }
}

public class UpdateVoucherRequestValidator : AbstractValidator<UpdateVoucherRequest>
{
    public UpdateVoucherRequestValidator()
    {
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.MinOrderAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UsageLimit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PerUserLimit).GreaterThanOrEqualTo(0);
    }
}
