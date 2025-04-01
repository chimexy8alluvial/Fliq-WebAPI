using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.GetAllEvents
{
    public record GetAllEventsTicketsQuery(
        PaginationRequest PaginationRequest = default!,
        string? Category = null,
        string? StatusFilter = null, 
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        string? Location = null) : IRequest<ErrorOr<List<GetEventsTicketsResult>>>;

    public class GetAllEventsTicketsQueryHandler : IRequestHandler<GetAllEventsTicketsQuery, ErrorOr<List<GetEventsTicketsResult>>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetAllEventsTicketsQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<GetEventsTicketsResult>>> Handle(GetAllEventsTicketsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Getting events with tickets for page {query.PaginationRequest.PageNumber} with page size {query.PaginationRequest.PageSize}");

                var request = new GetEventsTicketsListRequest
                {
                    PaginationRequest = query.PaginationRequest,
                    Category = query.Category,
                    StatusFilter = query.StatusFilter,
                    StartDate = query.StartDate,
                    EndDate = query.EndDate,
                    Location = query.Location
                };

                var results = await _ticketRepository.GetAllEventsTicketsForDashBoardAsync(request);

                _logger.LogInfo($"Got {results.Count()} events with tickets for page {query.PaginationRequest.PageNumber}");

                return results.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching events with tickets: {ex.Message}");
                return new List<Error>
                {
                    Error.Failure("GetEventsTicketsFailed", $"Failed to fetch events with tickets: {ex.Message}")
                };
            }
        }
    }
}