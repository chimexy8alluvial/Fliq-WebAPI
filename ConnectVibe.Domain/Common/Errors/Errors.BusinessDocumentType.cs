
using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class BusinessDocumentType
        {
            public static Error DuplicateName => Error.Conflict(
                code: "DocumentType.DuplicateName",
                description: "Document type already exists.");

            public static Error NotFound => Error.NotFound(
                code: "DocumentType.NotFound",
                description: "Document type not found.");

            public static Error InUse => Error.Conflict(
                code: "DocumentType.InUse",
                description: "Document type is in use and cannot be deleted.");
        }
    }
}
