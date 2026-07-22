using FluentValidation;
using PrintHub.Application.Features.Orders.Dtos;

namespace PrintHub.Application.Features.Orders.Validators;

/// <summary>Field-level validation for order placement; business rules live in the service.</summary>
public class PlaceOrderRequestValidator : AbstractValidator<PlaceOrderRequest>
{
    public PlaceOrderRequestValidator()
    {
        RuleFor(x => x.QuoteId).GreaterThan(0);

        RuleFor(x => x.Items).NotEmpty().WithMessage("An order must contain at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ServiceTypeId).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.PageCount).GreaterThan(0).When(i => i.PageCount.HasValue);
            item.RuleFor(i => i.EstimatedGrams).GreaterThan(0).When(i => i.EstimatedGrams.HasValue);
        });
    }
}

/// <summary>Cancellation reason is optional but bounded when supplied.</summary>
public class CancelOrderRequestValidator : AbstractValidator<CancelOrderRequest>
{
    public CancelOrderRequestValidator()
    {
        RuleFor(x => x.Reason).MaximumLength(500).When(x => x.Reason is not null);
    }
}
