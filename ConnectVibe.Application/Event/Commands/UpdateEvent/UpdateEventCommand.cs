using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.EventServices;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Event.Common;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Profile;
using Mapster;
using MapsterMapper;
using MediatR;
using System.Diagnostics;

namespace Fliq.Application.Event.Commands.UpdateEvent
{
    public class UpdateEventCommand : IRequest<ErrorOr<CreateEventResult>>
    {
        public int EventId { get; set; }
        public EventType? EventType { get; set; }
        public string? EventTitle { get; set; }
        public string? EventDescription { get; set; }
        public EventCategory? EventCategory { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Location? Location { get; set; }
        public int? Capacity { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public bool? SponsoredEvent { get; set; }
        public SponsoredEventDetail? SponsoredEventDetail { get; set; }
        public EventCriteria? EventCriteria { get; set; }
        public List<Ticket>? Tickets { get; set; }
        public int UserId { get; set; }
        public EventPaymentDetail? EventPaymentDetail { get; set; }
        public bool? InviteesException { get; set; }
        public List<EventMediaMapped>? MediaDocuments { get; set; }
        public List<EventInvitee>? EventInvitees { get; set; } = default!;
    }

    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, ErrorOr<CreateEventResult>>
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMediaServices _mediaServices;
        private readonly IEventRepository _eventRepository;
        private readonly ILocationService _locationService;
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;
        private readonly IEventService _eventService;
        private const string _eventDocument = "Event Documents";

        public UpdateEventCommandHandler(
            IMapper mapper,
            ILoggerManager logger,
            IUserRepository userRepository,
            IMediaServices mediaServices,
            IEventRepository eventRepository,
            ILocationService locationService,
            IMediator mediator,
            IEmailService emailService,
            IEventService eventService)
        {
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
            _mediaServices = mediaServices;
            _eventRepository = eventRepository;
            _locationService = locationService;
            _mediator = mediator;
            _emailService = emailService;
            _eventService = eventService;
        }

        public async Task<ErrorOr<CreateEventResult>> Handle(UpdateEventCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Updating Event with Id: {command.EventId}");
            var eventFromDb = _eventRepository.GetEventById(command.EventId);
            if (eventFromDb == null)
            {
                _logger.LogError($"Event with Id: {command.EventId} was not found.");
                return Errors.Event.EventNotFound;
            }

            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                _logger.LogError($"User with Id: {command.UserId} was not found.");
                return Errors.User.UserNotFound;
            }

            var eventToUpdate = command.Adapt(eventFromDb);

            if (command.MediaDocuments is not null)
            {
                eventToUpdate.Media.Clear();
                foreach (var photo in command.MediaDocuments)
                {
 
                    var mediaUrl = await _mediaServices.UploadMediaAsync(photo.DocFile, _eventDocument);
                    if (mediaUrl != null)
                    {
                        EventMedia eventMedia = new() { MediaUrl = mediaUrl, Title = photo.Title };
                        eventToUpdate.Media.Add(eventMedia);
                    }
                    else
                    {
                        return Errors.Document.InvalidDocument;
                    }

                }
            }
            if (command.Location is not null)
            {
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

                    eventToUpdate.Location = location;
                }
            }

            _eventRepository.Update(eventToUpdate);

            if(command.EventTitle != eventFromDb.EventTitle || command.StartDate != eventFromDb.StartDate || command.Location != eventFromDb.Location)
            {
                // Trigger Organizer Notification
                var organizerName = $"{user.FirstName} {user.LastName}";

                await _mediator.Publish(new EventCreatedEvent(
                    user.Id,
                    eventFromDb.Id,
                    user.Id,
                    organizerName,
                    Enumerable.Empty<int>(), // Organizer-only notification
                    "Event Updated",
                    $"Your event '{command.EventTitle}' has been successfully updated!",
                    false,
                    null,
                    null
                    
                ), cancellationToken);

                // Handle Invitees
                if (command.EventInvitees is not null)
                {
                    await SendInvitations(command.EventId, command.EventInvitees, user.Id, organizerName, command.EventTitle);
                }
            }

            return new CreateEventResult(eventToUpdate);
        }

        private async Task SendInvitations(int eventId, List<EventInvitee> invitees, int organizerId, string organizerName, string? eventTitle)
        {
            foreach (var invitee in invitees)
            {
                // Check if the user exists in the app
                var user = _userRepository.GetUserByEmail(invitee.Email);

                // Notify based on user type
                if (user != null)
                {
                    // Existing user: Send push notification and email

                    // Trigger notification
                    await _mediator.Publish(new EventCreatedEvent(
                        user.Id,
                        eventId,
                        organizerId,
                        organizerName,
                        inviteeIds: new List<int> { user.Id }, // Notification for this user
                        title: "Event Updated!",
                        message: $"The event '{eventTitle}' you were invited you to by  {organizerName} has been updated.",
                        actionUrl: null,
                        buttonText: "View Updated Event",
                        isUpdated: true
                    ));

                    //Send Email
                    await _emailService.SendEmailAsync(invitee.Email, "Event Updated", _eventService.GenerateEventCreationEmailContent(eventId, user.FirstName));
                }
                else
                {
                    // External user: Send email with optional registration link
                    await _emailService.SendEmailAsync(invitee.Email, "Event Updated", _eventService.GenerateEventCreationEmailContent(eventId, "", true));
                }
            }
        }

    }
}