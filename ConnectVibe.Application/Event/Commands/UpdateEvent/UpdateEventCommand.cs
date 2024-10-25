using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.DocumentServices;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Event.Common;
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
    }

    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, ErrorOr<CreateEventResult>>
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMediaServices _documentServices;
        private readonly IEventRepository _eventRepository;
        private readonly IImageService _imageService;
        private readonly ILocationService _locationService;

        public UpdateEventCommandHandler(
            IMapper mapper,
            ILoggerManager logger,
            IUserRepository userRepository,
            IMediaServices documentServices,
            IEventRepository eventRepository,
            IImageService imageService,
            ILocationService locationService)
        {
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
            _documentServices = documentServices;
            _eventRepository = eventRepository;
            _imageService = imageService;
            _locationService = locationService;
        }

        public async Task<ErrorOr<CreateEventResult>> Handle(UpdateEventCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Updating Event with Id: {command.EventId}");
            var eventFromDb = _eventRepository.GetEventById(command.EventId);
            if (eventFromDb == null)
            {
                _logger.LogError($"Event with Id: {command.EventId} was not found.");
                return Errors.User.UserNotFound;
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
                    if (Debugger.IsAttached)
                    {
                        var eventMediaUrl = await _documentServices.UploadEventMediaAsync(photo.DocFile);
                        if (eventMediaUrl != null)
                        {
                            EventMedia eventMedia = new() { MediaUrl = eventMediaUrl, Title = photo.Title };
                            eventToUpdate.Media.Add(eventMedia);
                        }
                    }
                    else
                    {
                        var mediaUrl = await _imageService.UploadMediaAsync(photo.DocFile);
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

            return new CreateEventResult(eventToUpdate);
        }
    }
}