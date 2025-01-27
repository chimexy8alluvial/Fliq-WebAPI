using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Wallet
        {
            public static Error AlreadyExists => Error.Conflict(
           code: "Wallet.AlreadyExists",
           description: "Wallet already exists");

            public static Error NotFound => Error.NotFound(
            code: "Wallet.NotFound",
            description: "Wallet not found");

            public static Error InsufficientBalance => Error.Failure(
                code: "Wallet.InsufficientBalance",
                description: "The wallet does not have sufficient balance.");

            public static Error UnableTodeduct => Error.Failure(
                code: "Wallet.UnableTodeduct",
                description: "Unable to deduct.");
        }
    }
}