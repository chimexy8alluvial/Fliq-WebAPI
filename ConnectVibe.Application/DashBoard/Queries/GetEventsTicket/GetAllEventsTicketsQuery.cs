using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event.Enums;
using MediatR;

namespace Fliq.Application.Tests.DashBoard.Queries.GetEventsTicket
{
    public record GetAllEventsTicketsQuery(
                    PaginationRequest PaginationRequest,
                    EventCategory? Category = null, 
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


                if (query.PaginationRequest == null)
                {
                    _logger.LogWarn("PaginationRequest is null, using default values");
                    query = query with { PaginationRequest = new PaginationRequest() };
                }

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

                _logger.LogInfo($"Got {results.Count} events with tickets for page {query.PaginationRequest.PageNumber}");

                return results.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching events with tickets: {ex.Message}");
                return Errors.Ticket.GetEventsTicketsFailed(ex.Message);
            }
        }
    }
}