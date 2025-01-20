using Fliq.Application.Common.Pagination;

namespace Fliq.Application.Games.Common
{
    public record GetQuestionsRequest
(
   int GameId, int PageNumber = 1, int PageSize = 10) : PaginationRequest(PageNumber, PageSize
);
}