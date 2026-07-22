using FluentValidation;
using PrintHub.Application.Features.Users.Dtos;

namespace PrintHub.Application.Features.Users.Validators;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(120);
        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .Matches(@"^[0-9+\-\s]+$").WithMessage("Phone number contains invalid characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        RuleFor(x => x.DefaultAddress).MaximumLength(300).When(x => x.DefaultAddress is not null);
        RuleFor(x => x.AvatarUrl).MaximumLength(500).When(x => x.AvatarUrl is not null);
    }
}
