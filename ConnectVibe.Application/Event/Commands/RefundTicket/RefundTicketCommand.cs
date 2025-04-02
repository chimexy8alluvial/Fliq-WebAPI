using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using MediatR;

namespace Fliq.Application.Commands
{
    public record RefundTicketCommand(int TicketId) : IRequest<ErrorOr<Unit>>;   

    public class RefundTicketCommandHandler : IRequestHandler<RefundTicketCommand, ErrorOr<Unit>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public RefundTicketCommandHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<Unit>> Handle(RefundTicketCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Processing refund request for TicketId: {command.TicketId}");

            try
            {                
                var ticket = _ticketRepository.GetTicketById(command.TicketId);
                if (ticket == null)
                {
                    _logger.LogWarn($"Ticket with TicketId {command.TicketId} not found.");
                    return Error.NotFound("TicketNotFound", $"Ticket with ID {command.TicketId} does not exist.");
                }

               
                if (ticket.IsRefunded)
                {
                    _logger.LogWarn($"Ticket with TicketId {command.TicketId} is already refunded.");
                    return Error.Conflict("TicketAlreadyRefunded", $"Ticket with ID {command.TicketId} has already been refunded.");
                }
           
                ticket.IsRefunded = true;
                 _ticketRepository.Update(ticket);

                _logger.LogInfo($"Successfully refunded TicketId: {command.TicketId}");
                return  Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error refunding TicketId {command.TicketId}: {ex.Message}");
                return Error.Failure("RefundTicketFailed", $"Failed to refund ticket: {ex.Message}");
            }
        }
    }
}