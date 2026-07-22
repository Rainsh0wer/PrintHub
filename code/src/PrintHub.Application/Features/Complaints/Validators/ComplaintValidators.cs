using FluentValidation;
using PrintHub.Application.Features.Complaints.Dtos;

namespace PrintHub.Application.Features.Complaints.Validators;

public class RaiseComplaintRequestValidator : AbstractValidator<RaiseComplaintRequest>
{
    public RaiseComplaintRequestValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Please describe the problem.")
            .MaximumLength(2000);
        RuleFor(x => x.AttachmentUrls).MaximumLength(2000).When(x => x.AttachmentUrls is not null);
    }
}

public class RespondComplaintRequestValidator : AbstractValidator<RespondComplaintRequest>
{
    public RespondComplaintRequestValidator()
    {
        RuleFor(x => x.RefundAmount).GreaterThan(0).When(x => x.RefundAmount.HasValue);
        RuleFor(x => x.ShopResponse).MaximumLength(1000).When(x => x.ShopResponse is not null);
    }
}

public class AdjudicateComplaintRequestValidator : AbstractValidator<AdjudicateComplaintRequest>
{
    public AdjudicateComplaintRequestValidator()
    {
        RuleFor(x => x.RefundAmount).GreaterThan(0).When(x => x.RefundAmount.HasValue);
        RuleFor(x => x.AdminRuling).MaximumLength(1000).When(x => x.AdminRuling is not null);
    }
}
