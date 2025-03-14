using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Pagination
        {
            public static Error PaginationInvalidParameters => Error.Forbidden(
            code: "Pagination.InvalidParameters",
            description: "PageNumber must be at least 1 and PageSize must be greater than 0.");
        }
    }
}
