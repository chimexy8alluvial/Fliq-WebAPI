using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.VipTicketCount
{
    public record GetEventVipTicketCountQuery(int EventId)
        : IRequest<ErrorOr<CountResult>>;

    public class GetEventVipTicketCountQueryHandler : IRequestHandler<GetEventVipTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetEventVipTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetEventVipTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                 _logger.LogInfo($"Fetching VIP ticket count for EventId: {query.EventId}");

                var count = await _ticketRepository.GetVipTicketCountAsync(query.EventId);
                _logger.LogInfo($"VIP Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching VIP ticket count for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetVipTicketCountFailed", $"Failed to fetch VIP ticket count: {ex.Message}");
            }
        }
    }
}

