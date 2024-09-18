using ErrorOr;

namespace ConnectVibe.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class User
        {
            public static Error UserNotFound => Error.NotFound(
    code: "User.NotFound",
    description: "User not found.");

            public static Error DuplicateEmail => Error.Conflict(
             code: "User.DuplicateEmail",
             description: "Email already in use");
        }
    }
}