using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.GetAllEvents
{
    public record GetAllEventsQuery(
                                    PaginationRequest PaginationRequest = default!,
                                    string? Category = null,
                                    DateTime? StartDate = null,
                                    DateTime? EndDate = null,
                                    string? Location = null) : IRequest<ErrorOr<List<GetEventsResult>>>;
    public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, ErrorOr<List<GetEventsResult>>>
    {
        private readonly IEventRepository _eventRepository;      
        private readonly ILoggerManager _logger;

        public GetAllEventsQueryHandler(IEventRepository eventRepository, ILoggerManager logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<GetEventsResult>>> Handle(GetAllEventsQuery query, CancellationToken cancellationToken)
        {

            _logger.LogInfo($"Getting events for page {query.PaginationRequest.PageNumber} with page size {query.PaginationRequest.PageSize}");

            var request = new GetEventsListRequest
            {
                PaginationRequest = query.PaginationRequest,
                Category = query.Category,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                Location = query.Location
            };

            var eventWithUsernames = await _eventRepository.GetAllEventsForDashBoardAsync(request);

            _logger.LogInfo($"Got {eventWithUsernames.Count()} events for page {query.PaginationRequest.PageNumber}");

            var results = eventWithUsernames.Select(eu =>
            {
                string status = DetermineEventStatus(eu.Event!.StartDate, eu.Event.EndDate);
                return new GetEventsResult(
                    EventTitle: eu.Event.EventTitle,
                    CreatedBy: eu.Username,
                    Status: status,
                    Attendees: eu.Event.Tickets?.Count ?? 0,
                    Type: eu.Event.SponsoredEvent ? "sponsored" : "free",
                    CreatedOn: eu.Event.DateCreated
                );
            }).ToList();


            return results;
        }

        private string DetermineEventStatus(DateTime startDate, DateTime endDate)
        {
            var now = DateTime.Now;

            if (startDate > now)
                return "Upcoming";
            if (startDate <= now && endDate >= now)
                return "Ongoing";
            return "Past";
        }

       
    }
}
