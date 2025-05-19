using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Payment
        {
            public static Error InvalidPayload => Error.Unexpected(
            code: "Payment.InvalidPayload",
            description: "Invalid payment payload.");

            public static Error FailedToProcess => Error.Unexpected(
            code: "Payment.FailedToProcess",
            description: "Failed to process payment.");
            
            public static Error InvalidPaymentTransaction => Error.Unexpected(
            code: "Payment.InvalidPaymentTransaction",
            description: "Invalid payment transaction.");

            public static Error PaymentNotFound => Error.NotFound(
            code: "Payment.PaymentFound",
            description: "Payment not found.");
            public static Error CurrencyNotFound => Error.NotFound(
            code: "Payment.CurrencyNotFound",
            description: "Currency not found.");
        }
    }
}