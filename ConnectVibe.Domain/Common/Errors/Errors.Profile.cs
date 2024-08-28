using ErrorOr;

namespace ConnectVibe.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Profile
        {
            public static Error ProfileNotFound => Error.NotFound(
                code: "Profile.NotFound",
                description: "Profile not found.");

            public static Error DuplicateProfile => Error.Conflict(
            code: "Profile.DuplicateProfile",
            description: "User already has a profile");
        }
    }
}