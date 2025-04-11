using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Common.Errors;
using MediatR;
using Error = ErrorOr.Error;

namespace Fliq.Application.DashBoard.Queries.RegularTicketCount
{
    public record GetEventRegularTicketCountQuery(int EventId)
        : IRequest<ErrorOr<CountResult>>;

    public class GetEventRegularTicketCountQueryHandler : IRequestHandler<GetEventRegularTicketCountQuery, ErrorOr<CountResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetEventRegularTicketCountQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger, IEventRepository eventRepository)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetEventRegularTicketCountQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Fetching regular ticket count for EventId: {query.EventId}");

                var eventFromDb = _eventRepository.GetEventById(query.EventId);
                if (eventFromDb == null)
                {
                    _logger.LogError($"Event with ID: {query.EventId} was not found.");
                    return Errors.Event.EventNotFound;
                }


                var count = await _ticketRepository.GetRegularTicketCountAsync(query.EventId);
            _logger.LogInfo($"Regular Ticket Count for EventId {query.EventId}: {count}");

            return new CountResult(count);
        }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching regular ticket count for EventId {query.EventId}: {ex.Message}");
                return Errors.Ticket.RegularCountFetchFailed(ex.Message);

            }
        }
    }
}