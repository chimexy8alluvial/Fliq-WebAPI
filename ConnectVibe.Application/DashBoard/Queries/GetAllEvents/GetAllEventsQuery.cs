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
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetAllEventsQueryHandler(IEventRepository eventRepository,IUserRepository userRepository, ILoggerManager logger)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<GetEventsResult>>> Handle(GetAllEventsQuery query, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Getting events for page {query.PaginationRequest.PageNumber} with page size {query.PaginationRequest.PageSize}");

            var request = new GetEventsListRequest
            {
                PaginationRequest = query.PaginationRequest,
                Category = query.Category,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                Location = query.Location
            };

            var events = await _eventRepository.GetAllEventsForDashBoardAsync(request);

            _logger.LogInfo($"Got {events.Count()} events for page {query.PaginationRequest.PageNumber}");

            var results =  events.Select( eventArgs =>
            {
                var user = _userRepository.GetUserById(eventArgs.UserId)!;

                var userName = $"{user.FirstName} {user.LastName}";

                string status = DetermineEventStatus(eventArgs.StartDate, eventArgs.EndDate);

                return new GetEventsResult(
                   EventTitle: eventArgs.EventTitle,
                    CreatedBy: userName,
                    Status:status,
                    Attendees: eventArgs.Tickets?.Count ?? 0,
                    EventCategory: eventArgs.EventCategory.ToString(),
                    CreatedOn: eventArgs.DateCreated
                );
            }).ToList();

            return  results;
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
