
using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Compliance
        {
            public static Error ComplianceNotFound => Error.NotFound(
            code: "Compliance.NotFound",
            description: "Compliance not found");

            public static Error RepositoryNotFound => Error.NotFound(
           code: "Repository.NotFound",
           description: "Repository not found");

        }
    }
}
