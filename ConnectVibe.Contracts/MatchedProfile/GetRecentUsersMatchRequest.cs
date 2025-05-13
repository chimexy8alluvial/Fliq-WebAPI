using Fliq.Domain.Enums;

namespace Fliq.Contracts.MatchedProfile
{
    public record GetRecentUsersMatchRequest
    {
        public int UserId { get; init; }
        public int Limit { get; init; }
        public MatchRequestStatus? Status { get; init; }
    }
}
