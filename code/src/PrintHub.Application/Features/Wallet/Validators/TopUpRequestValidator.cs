using FluentValidation;
using PrintHub.Application.Features.Wallet.Dtos;

namespace PrintHub.Application.Features.Wallet.Validators;

/// <summary>Bounds the top-up amount to a sensible range (VND).</summary>
public class TopUpRequestValidator : AbstractValidator<TopUpRequest>
{
    public TopUpRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(10_000).WithMessage("The minimum top-up is 10,000 VND.")
            .LessThanOrEqualTo(50_000_000).WithMessage("The maximum top-up is 50,000,000 VND.");
    }
}
