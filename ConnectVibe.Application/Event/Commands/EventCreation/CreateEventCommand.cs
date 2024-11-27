using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.DocumentServices;
using Fliq.Application.Common.Interfaces.Services.EventServices;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Event.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Profile;
using MapsterMapper;
using MediatR;
using System.Diagnostics;

namespace Fliq.Application.Event.Commands.EventCreation
{
    public class CreateEventCommand : IRequest<ErrorOr<CreateEventResult>>
    {
        public EventType EventType { get; set; }
        public string EventTitle { get; set; } = default!;
        public string EventDescription { get; set; } = default!;
        public EventCategory EventCategory { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Location Location { get; set; } = default!;
        public int Capacity { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public bool SponsoredEvent { get; set; } = default!;
        public SponsoredEventDetail? SponsoredEventDetail { get; set; } = default!;
        public EventCriteria EventCriteria { get; set; } = default!;
        public List<Ticket> Tickets { get; set; } = default!;
        public int UserId { get; set; } = default!;
        public EventPaymentDetail EventPaymentDetail { get; set; } = default!;
        public bool InviteesException { get; set; } = default!;
        public List<EventInvitee>? EventInvitees { get; set; } = default!;

        public List<EventMediaMapped> MediaDocuments { get; set; } = default!;
    }

    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, ErrorOr<CreateEventResult>>
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMediaServices _documentServices;
        private readonly IEventRepository _eventRepository;
        private readonly IImageService _imageService;
        private readonly ILocationService _locationService;
        private readonly IEventService _eventService;
        private readonly IEmailService _emailService;

        public CreateEventCommandHandler(IMapper mapper, ILoggerManager logger, IUserRepository userRepository,
            IMediaServices documentServices, IEventRepository eventRepository, IImageService imageService, ILocationService locationService, IEventService eventService, IEmailService emailService)
        {
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
            _documentServices = documentServices;
            _eventRepository = eventRepository;
            _imageService = imageService;
            _locationService = locationService;
            _eventService = eventService;
            _emailService = emailService;
        }

        public async Task<ErrorOr<CreateEventResult>> Handle(CreateEventCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Creating Event: {command.EventTitle}");
            await Task.CompletedTask;

            var user = _userRepository.GetUserById(command.UserId);

            if (user.IsDocumentVerified == false && command.EventType == EventType.Physical)
            {
                return Errors.User.DuplicateEmail;
            }

            var newEvent = _mapper.Map<Events>(command);

            foreach (var photo in command.MediaDocuments)
            {
                //Checking if the Application is running on a debugger mode
                if (Debugger.IsAttached)
                {
                    var eventMediaUrl = await _documentServices.UploadEventMediaAsync(photo.DocFile);
                    if (eventMediaUrl != null)
                    {
                        EventMedia eventMedia = new() { MediaUrl = eventMediaUrl, Title = photo.Title };
                        newEvent.Media.Add(eventMedia);
                    }
                }
                else
                {
                    var mediaUrl = await _imageService.UploadMediaAsync(photo.DocFile);
                    if (mediaUrl != null)
                    {
                        EventMedia eventMedia = new() { MediaUrl = mediaUrl, Title = photo.Title };
                        newEvent.Media.Add(eventMedia);
                    }
                    else
                    {
                        //return Errors.Image.InvalidImage;
                        return Errors.Document.InvalidDocument;
                    }
                }
            }

            var locationResponse = await _locationService.GetAddressFromCoordinatesAsync(command.Location.Lat, command.Location.Lng);
            if (locationResponse is not null)
            {
                LocationDetail locationDetail = _mapper.Map<LocationDetail>(locationResponse);
                Location location = new Location()
                {
                    LocationDetail = locationDetail,
                    IsVisible = command.Location.IsVisible,
                    Lat = command.Location.Lat,
                    Lng = command.Location.Lng
                };

                newEvent.Location = location;
            }
            _eventRepository.Add(newEvent);
            if (command.EventInvitees is not null)
            {
                await SendInvitations(newEvent.Id, command.EventInvitees);
            }
            return new CreateEventResult(newEvent);
        }

        private async Task SendInvitations(int eventId, List<EventInvitee> invitees)
        {
            foreach (var invitee in invitees)
            {
                // Check if the user exists in the app
                var user = _userRepository.GetUserByEmail(invitee.Email);

                // Notify based on user type
                if (user != null)
                {
                    // Existing user: Send push notification and email
                    //Notification Implementation

                    await _emailService.SendEmailAsync(invitee.Email, "Event Invitation", _eventService.GenerateEventCreationEmailContent(eventId, user.FirstName));
                }
                else
                {
                    // External user: Send email with optional registration link
                    await _emailService.SendEmailAsync(invitee.Email, "Event Invitation", _eventService.GenerateEventCreationEmailContent(eventId, "", true));
                }
            }
        }
    }
}