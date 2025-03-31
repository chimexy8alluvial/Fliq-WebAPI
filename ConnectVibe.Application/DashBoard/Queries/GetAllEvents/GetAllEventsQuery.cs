using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event.Enums;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.GetAllEvents
{
    public record GetAllEventsQuery(
                                    PaginationRequest PaginationRequest = default!,
                                    string? Category = null,
                                    EventStatus? Status = null,
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

            try
            {
                _logger.LogInfo($"Getting events for page {query.PaginationRequest.PageNumber} with page size {query.PaginationRequest.PageSize}");

                var request = new GetEventsListRequest
                {
                    PaginationRequest = query.PaginationRequest,
                    Category = query.Category,
                    Status = query.Status,
                    StartDate = query.StartDate,
                    EndDate = query.EndDate,
                    Location = query.Location
                };

                var results = await _eventRepository.GetAllEventsForDashBoardAsync(request);

                _logger.LogInfo($"Got {results.Count()} events for page {query.PaginationRequest.PageNumber}");

                return results.ToList(); // Implicit conversion to ErrorOr success case
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching events: {ex.Message}");
                return new List<Error> // Implicit conversion to ErrorOr error case
                {
                    Error.Failure("GetEventsFailed", $"Failed to fetch events: {ex.Message}")
                };
            }
        }
    }
}
            
    
