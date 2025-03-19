using Fliq.Application.Common.Pagination;
using System.Linq;
using System.Threading.Tasks;

namespace Fliq.Application.DashBoard.Common
{public class GetEventsListRequest
{
    public PaginationRequest? PaginationRequest { get; set; }
    public string? Category { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
}
}