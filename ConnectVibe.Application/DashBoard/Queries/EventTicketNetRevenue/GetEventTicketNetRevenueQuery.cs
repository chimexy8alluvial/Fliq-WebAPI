using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using MediatR;

namespace Fliq.Application.DashBoard.Queries
{
    public record GetEventTicketNetRevenueQuery(int EventId) : IRequest<ErrorOr<decimal>>;

    public class GetEventTicketNetRevenueQueryHandler : IRequestHandler<GetEventTicketNetRevenueQuery, ErrorOr<decimal>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetEventTicketNetRevenueQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<decimal>> Handle(GetEventTicketNetRevenueQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching net revenue for EventId: {query.EventId}");

                var netRevenue = await _ticketRepository.GetEventTicketNetRevenueAsync(query.EventId);
                _logger.LogInfo($"Net revenue for EventId {query.EventId}: {netRevenue}");

                return netRevenue;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching net revenue for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetNetRevenueFailed", $"Failed to fetch net revenue: {ex.Message}");
            }
        }
    }
}
