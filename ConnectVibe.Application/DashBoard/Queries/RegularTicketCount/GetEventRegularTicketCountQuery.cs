using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.RegularTicketCount
{
    public record GetEventRegularTicketCountQuery(int EventId)
        : IRequest<ErrorOr<CountResult>>;

    public class GetEventRegularTicketCountQueryHandler : IRequestHandler<GetEventRegularTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetEventRegularTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetEventRegularTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching regular ticket count for EventId: {query.EventId}");

            var count = await _ticketRepository.GetRegularTicketCountAsync(query.EventId);
            _logger.LogInfo($"Regular Ticket Count for EventId {query.EventId}: {count}");

            return new CountResult(count);
        }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching regular ticket count for EventId {query.EventId}: {ex.Message}");
                return Error.Failure("GetRegularTicketCountFailed", $"Failed to fetch regular ticket count: {ex.Message}");
            }
        }
    }
}