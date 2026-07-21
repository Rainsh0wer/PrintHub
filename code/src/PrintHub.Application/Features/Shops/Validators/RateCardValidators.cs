using FluentValidation;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Shops.Validators;

public class AddRateCardEntryRequestValidator : AbstractValidator<AddRateCardEntryRequest>
{
    public AddRateCardEntryRequestValidator()
    {
        RuleFor(x => x.ServiceTypeId).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);      // BR-71
        RuleFor(x => x.SetupFee).GreaterThanOrEqualTo(0);       // BR-71
        RuleFor(x => x.MinQuantity).GreaterThanOrEqualTo(1);
        RuleFor(x => x.LeadTimeMinutes).GreaterThanOrEqualTo(0);
    }
}

public class UpdateRateCardEntryRequestValidator : AbstractValidator<UpdateRateCardEntryRequest>
{
    public UpdateRateCardEntryRequestValidator()
    {
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SetupFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinQuantity).GreaterThanOrEqualTo(1);
        RuleFor(x => x.LeadTimeMinutes).GreaterThanOrEqualTo(0);
    }
}

public class AddPriceRuleRequestValidator : AbstractValidator<AddPriceRuleRequest>
{
    public AddPriceRuleRequestValidator()
    {
        RuleFor(x => x.OptionKey).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Multiplier).GreaterThan(0);               // BR-71
        RuleFor(x => x.FlatExtra).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxQuantity).GreaterThanOrEqualTo(x => x.MinQuantity)
            .When(x => x.MinQuantity.HasValue && x.MaxQuantity.HasValue)
            .WithMessage("Tier max quantity must be greater than or equal to min quantity.");
    }
}
