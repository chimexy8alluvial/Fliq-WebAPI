

namespace Fliq.Application.Common.Pagination
{
    public record PaginationResponse<T>(
        IEnumerable<T>? Data,
        int TotalCount,
        int PageNumber,
        int PageSize);
}
