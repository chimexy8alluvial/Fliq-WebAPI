using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using MediatR;

namespace Fliq.Application.DashBoard.Queries
{
    public record GetEventTicketGrossRevenueQuery(int EventId) : IRequest<ErrorOr<decimal>>;

    public class GetEventTicketGrossRevenueQueryHandler : IRequestHandler<GetEventTicketGrossRevenueQuery, ErrorOr<decimal>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetEventTicketGrossRevenueQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<decimal>> Handle(GetEventTicketGrossRevenueQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching gross revenue for EventId: {query.EventId}");

                var revenue = await _ticketRepository.GetEventTicketGrossRevenueAsync(query.EventId);
                _logger.LogInfo($"Gross revenue for EventId {query.EventId}: {revenue}");

                return revenue;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching gross revenue for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetGrossRevenueFailed", $"Failed to fetch gross revenue: {ex.Message}");
            }
        }
    }
}