using FluentValidation;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Shops.Validators;

public class ShopApplicationRequestValidator : AbstractValidator<ShopApplicationRequest>
{
    public ShopApplicationRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Length(2, 200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.AddressLine).NotEmpty().MaximumLength(300);
        RuleFor(x => x.District).NotEmpty().MaximumLength(100);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).MaximumLength(20)
            .Matches(@"^0\d{9}$").When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Please enter a valid phone number.");

        // BR-69: closing time must be later than opening time.
        RuleFor(x => x.CloseTime).GreaterThan(x => x.OpenTime)
            .WithMessage("Closing time must be later than opening time.");
    }
}

public class ReasonRequestValidator : AbstractValidator<ReasonRequest>
{
    public ReasonRequestValidator()
    {
        // BR-94: a reason is mandatory for reject/suspend/lock actions.
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000)
            .WithMessage("A reason is required for this action.");
    }
}
