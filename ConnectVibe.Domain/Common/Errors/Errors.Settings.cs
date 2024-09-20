using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Settings
        {
            public static Error SettingsNotFound => Error.NotFound(
                code: "Setting.NotFound",
                description: "Setting not found.");
        }
    }
}