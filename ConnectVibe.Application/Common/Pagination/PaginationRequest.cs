

namespace Fliq.Application.Common.Pagination
{
    public record PaginationRequest(int PageNumber = 1, int PageSize = 20)
    {
        public int PageNumber { get; set; } = PageNumber < 1 ? 1 : PageNumber;
        public int PageSize { get; set; } = PageSize < 1 ? 1 : PageSize > 50 ? 50 : PageSize;
    }
}
