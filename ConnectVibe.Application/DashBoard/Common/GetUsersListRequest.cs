using Fliq.Application.Common.Pagination;

namespace Fliq.Application.DashBoard.Common
{
    public class GetUsersListRequest
    {     
        public PaginationRequest? PaginationRequest { get; set; } 
        public bool? HasSubscription { get; set; } 
        public DateTime? ActiveSince { get; set; }
        public string? RoleName { get; set; }

    }
}
