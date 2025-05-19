

using Fliq.Contracts.Subscriptions;
using FluentValidation;

namespace Fliq.Application.Subscriptions.Commands
{
    public class CreateSubscriptionPlanRequestDtoValidator : AbstractValidator<CreateSubscriptionPlanRequestDto>
    {
        public CreateSubscriptionPlanRequestDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required.")
                .MaximumLength(50).WithMessage("ProductId must not exceed 50 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
