
using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class MatchRequest
        {
            public static Error DuplicateRequest => Error.Conflict(
                 code: "MatchRequest.Exists",
                 description: "Match request already exists.");

            public static Error RequestNotFound => Error.NotFound(
                code: "MatchRequest.NotFound",
                description: "Match request not found.");

            public static Error UnauthorizedAttempt => Error.Unauthorized(
               code: "MatchRequest.UnauthorizedAction",
               description: "Unauthorized action on Match request.");

            public static Error AlreadyAccepted => Error.Conflict(
              code: "MatchRequest.AlreadyAccepted",
              description: "Match request already accepted.");

            public static Error AlreadyRejected => Error.Conflict(
              code: "MatchRequest.AlreadyRejected",
              description: "Match request already rejected.");

        }
    }
}
