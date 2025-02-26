
using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Dating
        {
            public static Error DuplicateBlindDateCategory => Error.Conflict(
            code: "BlindDateCategory.AlreadyExist",
            description: "Blind Date Category already exists");

            public static Error AlreadyJoined => Error.Conflict(
          code: "BlindDate.AlreadyJoined",
          description: "Blind Date Already Joined");

            public static Error BlindDateSessionEnded => Error.Custom( 444,
          code: "BlindDate.SessionEnded",
          description: "Blind Date session ended");

            public static Error BlindDateCategoryNotFound => Error.NotFound(
            code: "BlindDateCategory.NotFound",
            description: "Blind Date Category not found");

            public static Error BlindDateNotFound => Error.NotFound(
            code: "BlindDate.NotFound",
            description: "Blind Date not found");

            public static Error BlindDateFull => Error.Forbidden(
            code: "BlindDate.AlreadyFull",
            description: "Blind Date already full");

        }
    }
}
