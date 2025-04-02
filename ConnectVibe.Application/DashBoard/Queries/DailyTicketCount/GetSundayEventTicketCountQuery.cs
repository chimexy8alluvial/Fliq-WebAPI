using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.DailyTicketCount
{
    public record GetSundayEventTicketCountQuery(int EventId, TicketType? TicketType = null)
        : IRequest<ErrorOr<CountResult>>;

    public class GetSundayEventTicketCountQueryHandler : IRequestHandler<GetSundayEventTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetSundayEventTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetSundayEventTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching Sunday ticket count for EventId: {query.EventId}, TicketType: {query.TicketType?.ToString() ?? "All"}");

                var count = await _ticketRepository.GetSundayTicketCountAsync(query.EventId, query.TicketType);
                _logger.LogInfo($"Sunday Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Sunday ticket count for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetSundayTicketCountFailed", $"Failed to fetch Sunday ticket count: {ex.Message}");
            }
        }
    }
}