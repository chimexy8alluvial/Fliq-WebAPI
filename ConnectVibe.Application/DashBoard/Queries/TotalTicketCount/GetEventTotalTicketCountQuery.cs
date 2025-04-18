using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.TotalTicketCount
{
    public record GetEventTotalTicketCountQuery(int EventId)
        : IRequest<ErrorOr<CountResult>>;
    public class GetEventTotalTicketCountQueryHandler : IRequestHandler<GetEventTotalTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetEventTotalTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetEventTotalTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching total ticket count for EventId: {query.EventId}");

                var count = await _ticketRepository.GetTotalTicketCountAsync(query.EventId);
                _logger.LogInfo($"Total Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching total ticket count for EventId {query.EventId}: {ex.Message}");
                return Errors.Ticket.GetTotalTicketCountFailed( ex.Message);
            }
        }
    }
}
