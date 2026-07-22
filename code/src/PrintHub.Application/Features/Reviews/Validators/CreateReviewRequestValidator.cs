using FluentValidation;
using PrintHub.Application.Features.Reviews.Dtos;

namespace PrintHub.Application.Features.Reviews.Validators;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.Comment).MaximumLength(1000).When(x => x.Comment is not null);
    }
}
