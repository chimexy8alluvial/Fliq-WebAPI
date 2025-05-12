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

            public static Error InvalidDocumentType => Error.Validation(
                code : "Document.InvalidType", 
                description: "The specified document type does not exist.");

            public static Error AlreadyExists => Error.Validation(
                code: "Document.AlreadyExists",
                description: "The business Identification document already exists for the user");

            public static Error MissingFront => Error.Validation(
                code: "Document.MissingFront",
                description: "FrontPage is required.");
        }
    }
}
