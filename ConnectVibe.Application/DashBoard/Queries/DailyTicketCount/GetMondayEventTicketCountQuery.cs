using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.DailyTicketCount
{
    public record GetMondayEventTicketCountQuery(int EventId, TicketType? TicketType = null)
        : IRequest<ErrorOr<CountResult>>;
    public class GetMondayEventTicketCountQueryHandler : IRequestHandler<GetMondayEventTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetMondayEventTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetMondayEventTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching Monday ticket count for EventId: {query.EventId}, TicketType: {query.TicketType?.ToString() ?? "All"}");

                var count = await _ticketRepository.GetMondayTicketCountAsync(query.EventId, query.TicketType);
                _logger.LogInfo($"Monday Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching monday ticket count for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetMondayTicketCountFailed", $"Failed to fetch other ticket count: {ex.Message}");
            }
        }
    }
}