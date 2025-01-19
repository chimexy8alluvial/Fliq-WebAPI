using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Stake
        {
            public static readonly Error NotFound =
                Error.NotFound("Stake.NotFound", "The stake was not found.");

            public static readonly Error InvalidRecipient =
                Error.Validation("Stake.InvalidRecipient", "The user is not the intended recipient of this stake.");

            public static readonly Error AlreadyAccepted =
                Error.Validation("Stake.AlreadyAccepted", "The stake has already been accepted.");
        }
    }
}