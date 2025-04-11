using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Common.Errors;
using MediatR;
using ErrorOr;
using Fliq.Domain.Entities.Event;

namespace Fliq.Application.DashBoard.Queries.DailyTicketCount
{
    public record GetWeeklyEventTicketCountQuery(
        int EventId,
        DateTime? StartDate,
        DateTime? EndDate,
        TicketType? TicketType = null
    ) : IRequest<ErrorOr<WeeklyCountResult>>;

    public class GetWeeklyEventTicketCountQueryHandler : IRequestHandler<GetWeeklyEventTicketCountQuery, ErrorOr<WeeklyCountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetWeeklyEventTicketCountQueryHandler(
            ITicketRepository ticketRepository,
            ILoggerManager logger,
            IEventRepository eventRepository)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<ErrorOr<WeeklyCountResult>> Handle(GetWeeklyEventTicketCountQuery query, CancellationToken cancellationToken)
        {
            var eventFromDb =  _eventRepository.GetEventById(query.EventId);
            if (eventFromDb == null)
            {
                _logger.LogError($"Event with ID: {query.EventId} was not found.");
                return Errors.Event.EventNotFound;
            }

            try
            {
                var dailyCounts = await _ticketRepository.GetWeeklyTicketCountAsync(
                    query.EventId,
                    query.StartDate,
                    query.EndDate,
                    query.TicketType);

                _logger.LogInfo($"Weekly Ticket Counts for EventId {query.EventId}: {FormatCounts(dailyCounts)}");
                return new WeeklyCountResult(dailyCounts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching weekly ticket counts for EventId {query.EventId}: {ex.Message}");
                return Errors.Ticket.WeeklyCountFetchFailed(ex.Message);
            }
        }

        private static string FormatCounts(Dictionary<DayOfWeek, int> counts) =>
            string.Join(", ", counts.Select(kv => $"{kv.Key}: {kv.Value}"));
    }
}