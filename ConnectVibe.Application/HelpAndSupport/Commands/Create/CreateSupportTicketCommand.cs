using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Contracts.HelpAndSupport;
using Fliq.Domain.Entities.HelpAndSupport;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.HelpAndSupport.Commands.Create
{
    public class CreateSupportTicketCommand : IRequest<ErrorOr<CreateSupportTicketResponse>>
    {
        public string Title { get; set; } = default!;
        public int RequesterId { get; set; }
        public string RequesterName { get; set; } = default!;
        public HelpRequestType RequestType { get; set; }
    }

    public class CreateSupportTicketCommandHandler : IRequestHandler<CreateSupportTicketCommand, ErrorOr<CreateSupportTicketResponse>>
    {
        private readonly ISupportTicketRepository _repository;
        private readonly ILoggerManager _logger;

        public CreateSupportTicketCommandHandler(ISupportTicketRepository repository, ILoggerManager loggerManager)
        {
            _repository = repository;
            _logger = loggerManager;
        }

        public async Task<ErrorOr<CreateSupportTicketResponse>> Handle(CreateSupportTicketCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Help request command received");
            await Task.CompletedTask;
            var ticket = new SupportTicket
            {
                Title = command.Title,
                RequesterId = command.RequesterId,
                RequesterName = command.RequesterName,
                RequestType = command.RequestType,
                RequestStatus = HelpRequestStatus.Pending
            };
            var res = await _repository.CreateTicketAsync(ticket);
            _logger.LogInfo($"Help request created with ID {res.TicketId}");
            return new CreateSupportTicketResponse(res.TicketId);
        }
    }
}