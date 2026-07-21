using FluentValidation;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Shops.Validators;

public class GrantStaffRequestValidator : AbstractValidator<GrantStaffRequest>
{
    public GrantStaffRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Position).MaximumLength(100);
    }
}
