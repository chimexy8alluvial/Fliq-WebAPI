using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.OtherTicketCount
{
    public record GetEventOtherTicketCountQuery(int EventId)
        : IRequest<ErrorOr<CountResult>>;
    public class GetEventOtherTicketCountQueryHandler : IRequestHandler<GetEventOtherTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetEventOtherTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetEventOtherTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching Other ticket count for EventId: {query.EventId}");

                var count = await _ticketRepository.GetOtherTicketCountAsync(query.EventId);
                _logger.LogInfo($"Other Ticket Count for EventId {query.EventId}: {count}");

                return new CountResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching other ticket count for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetOtherTicketCountFailed", $"Failed to fetch other ticket count: {ex.Message}");
            }
        }
    }
}