using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class User
        {
            public static Error UserNotFound => Error.NotFound(
            code: "User.NotFound",
            description: "User not found.");

           public static Error UserAlreadyDeleted => Error.NotFound(
            code: "User.AlreadyDeleted ",
            description: "User Already Deleted ");

            public static Error DuplicateEmail => Error.Conflict(
             code: "User.DuplicateEmail",
             description: "Email already in use");

            public static Error DocumentUnverified => Error.Conflict(
            code: "User.DocumentUnverified",
            description: "User's Document is Unverified");
        }
    }
}