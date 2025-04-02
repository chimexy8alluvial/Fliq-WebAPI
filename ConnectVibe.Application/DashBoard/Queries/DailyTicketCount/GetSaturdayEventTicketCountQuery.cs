using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.DailyTicketCount
{
    public record GetSaturdayEventTicketCountQuery(int EventId, TicketType? TicketType = null)
        : IRequest<ErrorOr<CountResult>>;

    public class GetSaturdayEventTicketCountQueryHandler : IRequestHandler<GetSaturdayEventTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetSaturdayEventTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetSaturdayEventTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching Saturday ticket count for EventId: {query.EventId}, TicketType: {query.TicketType?.ToString() ?? "All"}");

                var count = await _ticketRepository.GetSaturdayTicketCountAsync(query.EventId, query.TicketType);
                _logger.LogInfo($"Saturday Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Saturday ticket count for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetSaturdayTicketCountFailed", $"Failed to fetch Saturday ticket count: {ex.Message}");
            }
        }
    }
}