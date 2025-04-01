using Fliq.Application.Common.Pagination;

namespace Fliq.Application.DashBoard.Common
{
    public class GetEventsTicketsListRequest
    {
        public PaginationRequest PaginationRequest { get; set; } = default!;
        public string? Category { get; set; }
        public string? StatusFilter { get; set; } // New nullable string filter for "SoldOut" or "Cancelled"
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
    }
}

