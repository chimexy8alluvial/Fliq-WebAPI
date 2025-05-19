

using FluentValidation;

namespace Fliq.Application.Subscriptions.Commands
{
    public class AddSubscriptionPlanPriceCommandValidator : AbstractValidator<AddSubscriptionPlanPriceCommand>
    {
        public AddSubscriptionPlanPriceCommandValidator()
        {
            RuleFor(x => x.SubscriptionPlanId)
                .GreaterThan(0)
                .WithMessage("SubscriptionPlanId must be greater than 0.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required.")
                .Length(3)
                .WithMessage("Currency must be a 3-letter ISO code (e.g., USD, EUR).")
                .Matches("^[A-Z]{3}$")
                .WithMessage("Currency must be uppercase 3-letter code.");

            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("Country is required.")
                .MaximumLength(4);

            RuleFor(x => x.EffectiveFrom)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("EffectiveFrom must be a future date and time.");

            RuleFor(x => x.Store)
                .MaximumLength(100)
                .WithMessage("Store name must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Store));
        }
    }
}
