using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.HelpAndSupport;
using MediatR;

namespace Fliq.Application.HelpAndSupport.Commands.AddMessage
{
    public class AddMessageToTicketCommand : IRequest<ErrorOr<int>>
    {
        public string SupportTicketId { get; set; } = default!; // Use SupportTicketId instead of TicketId
        public int SenderId { get; set; }
        public string SenderName { get; set; } = default!;
        public string Content { get; set; } = default!;
    }

    public class AddMessageToTicketCommandHandler : IRequestHandler<AddMessageToTicketCommand, ErrorOr<int>>
    {
        private readonly ISupportTicketRepository _repository;
        private readonly ILoggerManager _logger;

        public AddMessageToTicketCommandHandler(ISupportTicketRepository repository, ILoggerManager loggerManager)
        {
            _repository = repository;
            _logger = loggerManager;
        }

        public async Task<ErrorOr<int>> Handle(AddMessageToTicketCommand command, CancellationToken cancellationToken)
        {
            var ticket = await _repository.GetTicketBySupportTicketIdAsync(command.SupportTicketId);
            if (ticket == null)
            {
                _logger.LogError($"Ticket with SupportTicketId {command.SupportTicketId} not found.");
                return Error.NotFound("Ticket not found.");
            }

            var message = new HelpMessage
            {
                SenderId = command.SenderId,
                SenderName = command.SenderName,
                Content = command.Content
            };

            await _repository.AddMessageToTicketAsync(ticket.Id, message);
            _logger.LogInfo($"Message added to ticket with SupportTicketId {command.SupportTicketId}.");

            return message.Id;
        }
    }
}