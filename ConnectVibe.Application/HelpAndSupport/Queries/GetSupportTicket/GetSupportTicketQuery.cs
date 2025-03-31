using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.HelpAndSupport;
using MediatR;

namespace Fliq.Application.HelpAndSupport.Queries.GetSupportTicket
{
    public class GetSupportTicketQuery : IRequest<ErrorOr<SupportTicket>>
    {
        public string SupportTicketId { get; set; } = default!; // Use SupportTicketId instead of TicketId
    }

    public class GetSupportTicketQueryHandler : IRequestHandler<GetSupportTicketQuery, ErrorOr<SupportTicket>>
    {
        private readonly ISupportTicketRepository _repository;
        private readonly ILoggerManager _logger;

        public GetSupportTicketQueryHandler(ISupportTicketRepository repository, ILoggerManager loggerManager)
        {
            _repository = repository;
            _logger = loggerManager;
        }

        public async Task<ErrorOr<SupportTicket>> Handle(GetSupportTicketQuery query, CancellationToken cancellationToken)
        {
            var ticket = await _repository.GetTicketBySupportTicketIdAsync(query.SupportTicketId);
            if (ticket == null)
            {
                _logger.LogError($"Ticket with SupportTicketId {query.SupportTicketId} not found.");
                return Error.NotFound("Ticket not found.");
            }

            return ticket;
        }
    }
}