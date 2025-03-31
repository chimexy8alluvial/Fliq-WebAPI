using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Common;
using Fliq.Domain.Common.Errors;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Event.Queries.GetTicket
{
    public record GetTicketQuery(int TicketId) : IRequest<ErrorOr<CreateTicketResult>>;

    public class GetTicketQueryHandler : IRequestHandler<GetTicketQuery, ErrorOr<CreateTicketResult>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetTicketQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CreateTicketResult>> Handle(GetTicketQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Get Ticket: {query.TicketId}");
            var ticket = _ticketRepository.GetTicketById(query.TicketId);
            if (ticket == null)
            {
                _logger.LogError($"Ticket not found: {query.TicketId}");
                return Errors.Ticket.TicketNotFound;
            }

            return new CreateTicketResult(ticket);
        }
    }
}