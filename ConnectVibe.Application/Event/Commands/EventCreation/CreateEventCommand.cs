using System.Diagnostics;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Common.Interfaces.Services.ImageServices;
using ConnectVibe.Application.Common.Interfaces.Services.LocationServices;
using ConnectVibe.Domain.Entities.Profile;
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.DocumentServices;
using Fliq.Application.Event.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Event.Commands.EventCreation
{
    public class CreateEventCommand : IRequest<ErrorOr<CreateEventResult>>
    {
        public int Id { get; set; }
        public EventType EventType { get; set; }
        public string EventTitle { get; set; } = default!;
        public string EventDescription { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Location Location { get; set; } = default!;
        public int Capacity { get; set; }
        public int UserId { get; set; } = default!;
        public List<EventMediaMapped> Docs { get; set; } = default!;
        public string StartAge { get; set; } = default!;
        public string EndAge { get; set; } = default!;
        public string EventCategory { get; set; } = default!;
        public bool SponsoredEvent { get; set; }
        public SponsoredEventDetail SponsoredEventDetail { get; set; } = default!;
        public EventCriteria EventCriteria { get; set; } = default!;
        public List<TicketType> TicketType { get; set; } = default!;
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


        public CreateEventCommandHandler(IMapper mapper, ILoggerManager logger, IUserRepository userRepository,
            IMediaServices documentServices, IEventRepository eventRepository, IImageService imageService, ILocationService locationService)
        {
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
            _documentServices = documentServices;
            _eventRepository = eventRepository;
            _imageService = imageService;
            _locationService = locationService;
        }

        public async Task<ErrorOr<CreateEventResult>> Handle(CreateEventCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                return Errors.User.DuplicateEmail;
            }

            if (user.IsDocumentVerified == false && command.EventType == EventType.Physical)
            {
                return Errors.User.DuplicateEmail;
            }

            var newEvent = _mapper.Map<Events>(command);
            
            foreach (var photo in command.Docs)
            {
                //Checking if the Application is running on a debugger mode
                if (Debugger.IsAttached)
                {
                    var eventMediaUrl = await _documentServices.UploadEventMediaAsync(photo.DocFile);
                    if(eventMediaUrl != null) 
                    {
                        EventMediaa eventMedia = new() {MediaUrl = eventMediaUrl, Title = photo.Title };
                        newEvent.Media.Add(eventMedia);
                    }
                }
                else
                {
                    var mediaUrl = await _imageService.UploadMediaAsync(photo.DocFile);
                    if (mediaUrl != null)
                    {
                        EventMediaa eventMedia = new() { MediaUrl = mediaUrl, Title = photo.Title };
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

            return new CreateEventResult(newEvent);
        }
    }
}
