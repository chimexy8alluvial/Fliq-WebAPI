using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Domain.Entities.Profile;
using ErrorOr;
using Fliq.Application.Authentication.Common.Event;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.DocumentServices;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Event.Commands.EventCreation
{
    public class CreateEventCommand : IRequest<ErrorOr<CreateEventResult>>
    {
        public int Id { get; set; }
        public EventType EventType { get; set; }
        public string eventTitle { get; set; } = default!;
        public string eventDescription { get; set; } = default!;
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        //public TimeZoneInfo timeZone { get; set; } = default!;
        public Location Location { get; set; } = default!;
        public int capacity { get; set; }
        public string optional { get; set; } = default!;
        public int UserId { get; set; } = default!;
        public List<EvtDocumentDto> Docs { get; set; } = default!;
    }
    public enum EventType
    {
        Physical,
        Live
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

            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                return Errors.User.DuplicateEmail;
            }

            var newEvent = _mapper.Map<Events>(command);
            foreach (var docss in command.Docs)
            {
                var documentUrl = await _documentServices.UploadDocumentAsync(docss.Documentfile);
                if (documentUrl != null)
                {
                    EventDocument eventDocument = new() { DocumentUrl = documentUrl, Title = docss.Title };
                    newEvent.Docs.Add(eventDocument);
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
