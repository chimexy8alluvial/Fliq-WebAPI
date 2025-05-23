
using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Content
        {
            public static Error ContentNotFound => Error.NotFound(
            code: "Content.NotFound",
            description: "Content not found");

            public static Error ContentAlreadyRejected => Error.Conflict(
            code: "Content.ContentAlreadyRejected;",
            description: "This Content Request is already Rejected");

            public static Error ContentAlreadyApproved => Error.Conflict(
            code: "Content.ContentAlreadyApproved;",
            description: "This Content Request is already Approved");

            public static Error RepositoryNotFound => Error.NotFound(
           code: "Repository.NotFound",
           description: "Repository not found");

        }
    }
}
