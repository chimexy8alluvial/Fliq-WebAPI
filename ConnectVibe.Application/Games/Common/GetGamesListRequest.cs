using Fliq.Domain.Enums;


namespace Fliq.Application.Games.Common
{
    public class GetGamesListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime? DatePlayedFrom { get; set; }
        public DateTime? DatePlayedTo { get; set; }
        public int? Status { get; set; }
    };
}
