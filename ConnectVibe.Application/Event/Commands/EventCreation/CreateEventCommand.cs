using Fliq.Domain.Entities.Event;
using Fliq.Application.Authentication.Common.Event;
using MediatR;
using Fliq.Application.Event.Common;
using MapsterMapper;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.DocumentServices;
using Fliq.Application.Common.Interfaces.Persistence;
using ErrorOr;
using ConnectVibe.Domain.Common.Errors;

namespace Fliq.Application.Event.Commands.EventCreation
{
    public class CreateEventCommand : IRequest<ErrorOr<CreateEventResult>>
    {
        public int EventId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string EventType { get; set; } = default!;
        public List<EventDocumentsMapp> Docs{ get; set; } = default!;
    }

    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, ErrorOr<CreateEventResult>>
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IDocumentServices _documentServices;
        private readonly IEventRepository _eventRepository;

        public CreateEventCommandHandler(IMapper mapper, ILoggerManager logger, IUserRepository userRepository,
            IDocumentServices documentServices, IEventRepository eventRepository)
        {
                _mapper = mapper;
                _logger = logger;
                _userRepository = userRepository;
                _documentServices = documentServices;
            _eventRepository = eventRepository;
        }

        public async Task<ErrorOr<CreateEventResult>> Handle(CreateEventCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var user = _userRepository.GetUserByEmail(command.Email);
            if (user == null)
            {
                return Errors.Profile.ProfileNotFound;
            }

            var newEvent = _mapper.Map<CreateEvent>(command);
            foreach(var docss in command.Docs)
            {
                var documentUrl = await _documentServices.UploadDocumentAsync(docss.DocFile);
                if (documentUrl != null)
                {
                    EventDocument eventDocument = new() { DocumentUrl = documentUrl, Title = docss.Title };
                    newEvent.document.Add(eventDocument);
                }
                else
                {
                    return Errors.Document.InvalidDocument;
                } 
            }
            _eventRepository.Add(newEvent);

            var createdEvent = _mapper.Map<CreateEventResult>(newEvent);

            return createdEvent;
        }
    }
}
