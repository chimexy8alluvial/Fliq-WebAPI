using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Profile
        {
            public static Error ProfileNotFound => Error.NotFound(
                code: "Profile.NotFound",
                description: "User profile not found.");

            public static Error DuplicateProfile => Error.Conflict(
            code: "Profile.DuplicateProfile",
            description: "User already has a profile");
        }
    }
}