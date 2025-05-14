using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Image
        {
            public static Error InvalidImage => Error.Failure(
             code: "Image.InvalidImage",
             description: "Image is not valid");

            public static Error UploadFailed => Error.Failure(
            code: "Image.UploadFailed",
            description: "Image upload failed. Try again");
        }
    }
}