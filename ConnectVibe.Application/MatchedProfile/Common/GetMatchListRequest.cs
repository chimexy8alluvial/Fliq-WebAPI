using Fliq.Application.Common.Pagination;
using Fliq.Domain.Enums;

namespace Fliq.Application.MatchedProfile.Common
{
    public class GetMatchListRequest
    {
        public int UserId { get; set; }

        public PaginationRequest PaginationRequest { get; set; } = default!;

        public MatchRequestStatus? MatchRequestStatus { get; set; }
    }
}