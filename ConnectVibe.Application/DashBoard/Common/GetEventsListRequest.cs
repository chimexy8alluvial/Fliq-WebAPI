using Fliq.Application.Common.Pagination;

namespace Fliq.Application.DashBoard.Common
{
    public class GetEventsListRequest
    {
        public PaginationRequest PaginationRequest { get; set; } = default!;
        public string? Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
    }
}
