using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Document
        {
            public static Error InvalidDocument => Error.Failure(
                code: "Document.InvalidDocument",
                description: "Document not valid");
        }
    }
}
