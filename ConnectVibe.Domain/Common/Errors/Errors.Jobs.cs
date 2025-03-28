using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Jobs
        {
            public static Error JobNotFound => Error.NotFound(
            code: "Job.NotFound",
            description: "Job not found.");

         
        }
    }
}