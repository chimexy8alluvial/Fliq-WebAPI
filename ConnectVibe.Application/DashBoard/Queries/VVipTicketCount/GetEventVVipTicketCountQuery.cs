using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.VVipTicketCount
{
    public record GetEventVVipTicketCountQuery(int EventId)
        : IRequest<ErrorOr<CountResult>>;
    public class GetEventVVipTicketCountQueryHandler : IRequestHandler<GetEventVVipTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetEventVVipTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetEventVVipTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching VVIP ticket count for EventId: {query.EventId}");

                var count = await _ticketRepository.GetVVipTicketCountAsync(query.EventId);
                _logger.LogInfo($"VVIP Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching VVIP ticket count for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetVVipTicketCountFailed", $"Failed to fetch VVIP ticket count: {ex.Message}");
            }
        }
    }
}