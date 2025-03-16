
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
            public static Error NoBlindDateCategoryName => Error.Failure(
          code: "BlindDateCategory.EmptyName",
          description: "Blind Date Category requires name");
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

            public static Error NotSessionCreator => Error.Forbidden(
            code: "BlindDate.NotSessionCreator",
            description: "User is not the Session Creator");

            public static Error BlindDateAlreadyStarted => Error.Conflict(
            code: "BlindDate.BlindDateAlreadyStarted;",
            description: "Session has already started");

           public static Error BlindDateAlreadyEnded => Error.Conflict(
           code: "BlindDate.BlindDateAlreadyEnded;",
           description: "Session has already ended");

            public static Error BlindDateCancelled => Error.Failure(
         code: "BlindDate.BlindDateCancelled;",
         description: "Session is cancelled");

            public static Error BlindDateNotOngoing => Error.Failure(
         code: "BlindDate.BlindDateNotOngoing;",
         description: "This Session is not ongoing");

        }
    }
}
