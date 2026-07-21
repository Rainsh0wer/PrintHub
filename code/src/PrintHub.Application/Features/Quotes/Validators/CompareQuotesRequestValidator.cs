using FluentValidation;
using PrintHub.Application.Features.Quotes.Dtos;

namespace PrintHub.Application.Features.Quotes.Validators;

public class CompareQuotesRequestValidator : AbstractValidator<CompareQuotesRequest>
{
    public CompareQuotesRequestValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("Please configure at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ServiceTypeId).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThanOrEqualTo(1);
        });
    }
}
