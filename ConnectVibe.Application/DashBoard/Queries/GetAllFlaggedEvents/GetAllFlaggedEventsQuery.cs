using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event.Enums;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.GetAllEvents
{
    public record GetAllFlaggedEventsQuery(
                                    PaginationRequest PaginationRequest = default!,
                                    string? Category = null,
                                     EventStatus? Status = null,
                                    DateTime? StartDate = null,
                                    DateTime? EndDate = null,
                                    string? Location = null) : IRequest<ErrorOr<List<GetEventsResult>>>;
    public class GetAllFlaggedEventsQueryHandler : IRequestHandler<GetAllFlaggedEventsQuery, ErrorOr<List<GetEventsResult>>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetAllFlaggedEventsQueryHandler(IEventRepository eventRepository, ILoggerManager logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<GetEventsResult>>> Handle(GetAllFlaggedEventsQuery query, CancellationToken cancellationToken)
        {
            try 
            { 

             _logger.LogInfo($"Getting flagged events for page {query.PaginationRequest.PageNumber} with page size {query.PaginationRequest.PageSize}");

            var request = new GetEventsListRequest
            {
                PaginationRequest = query.PaginationRequest,
                Category = query.Category,
                Status = query.Status,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                Location = query.Location
            };

            var results = await _eventRepository.GetAllFlaggedEventsForDashBoardAsync(request);

            _logger.LogInfo($"Got {results.Count()} flagged events for page {query.PaginationRequest.PageNumber}");


            return results.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching flagged events: {ex.Message}");
                return new List<Error> // Implicit conversion to ErrorOr error case
                {
                    Error.Failure("GetFlaggedEventsFailed", $"Failed to fetch events: {ex.Message}")
                };
            }
        }
    }
}
