using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using MediatR;
using Error = ErrorOr.Error;

namespace Fliq.Application.DashBoard.Queries
{
    public record GetEventTicketNetRevenueQuery(int EventId) : IRequest<ErrorOr<decimal>>;

    public class GetEventTicketNetRevenueQueryHandler : IRequestHandler<GetEventTicketNetRevenueQuery, ErrorOr<decimal>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetEventTicketNetRevenueQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger, IEventRepository eventRepository)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<ErrorOr<decimal>> Handle(GetEventTicketNetRevenueQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching net revenue for EventId: {query.EventId}");

                var eventFromDb = _eventRepository.GetEventById(query.EventId);
                if (eventFromDb == null)
                {
                    _logger.LogError($"Event with ID: {query.EventId} was not found.");
                    return Errors.Event.EventNotFound;
                }


                var netRevenue = await _ticketRepository.GetEventTicketNetRevenueAsync(query.EventId);
                _logger.LogInfo($"Net revenue for EventId {query.EventId}: {netRevenue}");

                return netRevenue;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching net revenue for EventId {query.EventId}: {ex.Message}");
                return Errors.Ticket.NetRevenueFetchFailed(ex.Message);
            }
        }
    }
}
