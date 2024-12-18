
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

        }
    }
}
