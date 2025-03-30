using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Application.DashBoard.Common
{
    public class GetEventsListRequest
    {
        public PaginationRequest? PaginationRequest { get; set; }
        public string? Category { get; set; }
        public EventStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
    }
}
