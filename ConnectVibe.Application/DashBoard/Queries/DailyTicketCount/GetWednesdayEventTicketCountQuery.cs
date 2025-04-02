using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.DailyTicketCount
{
    public record GetWednesdayEventTicketCountQuery(int EventId, TicketType? TicketType = null)
        : IRequest<ErrorOr<CountResult>>;

    public class GetWednesdayEventTicketCountQueryHandler : IRequestHandler<GetWednesdayEventTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetWednesdayEventTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetWednesdayEventTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching Wednesday ticket count for EventId: {query.EventId}, TicketType: {query.TicketType?.ToString() ?? "All"}");

                var count = await _ticketRepository.GetWednesdayTicketCountAsync(query.EventId, query.TicketType);
                _logger.LogInfo($"Wednesday Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Wednesday ticket count for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetWednesdayTicketCountFailed", $"Failed to fetch Wednesday ticket count: {ex.Message}");
            }
        }
    }
}