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
        }
    }
}