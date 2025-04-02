using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.DailyTicketCount
{
    public record GetTuesdayEventTicketCountQuery(int EventId, TicketType? TicketType = null)
        : IRequest<ErrorOr<CountResult>>;

    public class GetTuesdayEventTicketCountQueryHandler : IRequestHandler<GetTuesdayEventTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetTuesdayEventTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetTuesdayEventTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching Tuesday ticket count for EventId: {query.EventId}, TicketType: {query.TicketType?.ToString() ?? "All"}");

                var count = await _ticketRepository.GetTuesdayTicketCountAsync(query.EventId, query.TicketType);
                _logger.LogInfo($"Tuesday Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Tuesday ticket count for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetTuesdayTicketCountFailed", $"Failed to fetch Tuesday ticket count: {ex.Message}");
            }
        }
    }
}