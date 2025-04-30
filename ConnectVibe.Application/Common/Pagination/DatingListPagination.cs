
namespace Fliq.Application.Common.Pagination
{
    public record DatingListPagination(int PageNumber = 1, int PageSize = 10)
    {
        public int PageNumber { get; set; } = PageNumber < 1 ? 1 : PageNumber;
        public int PageSize { get; set; } = PageSize < 1 ? 1 : PageSize > 10 ? 10 : PageSize;
    }
}
