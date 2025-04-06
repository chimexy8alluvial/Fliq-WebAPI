using Fliq.Domain.Enums;


namespace Fliq.Application.Games.Common
{
    public class GetGamesListRequest
    {
        public int Page,
        int PageSize,
        DateTime? DatePlayedFrom,
        DateTime? DatePlayedTo,
        GameStatus? Status
    };
}
