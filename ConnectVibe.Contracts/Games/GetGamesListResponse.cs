using Fliq.Domain.Enums;

namespace Fliq.Contracts.Games
{
    public record GetGamesListResponse
    {
        public List<GamesListItem> List { get; }
        public int TotalCount { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public GetGamesListResponse(List<GamesListItem> list, int totalCount, int page, int pageSize)
        {
            List = list;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }

    public class GamesListItem
    {
        public int Id { get; set; }
        public string? GameTitle { get; set; }
        public string? Players { get; set; }
        public GameStatus Status { get; set; }
        public string? Stake { get; set; }
        public string? Winner { get; set; }
        public DateTime? DatePlayed { get; set; }
    }

}
