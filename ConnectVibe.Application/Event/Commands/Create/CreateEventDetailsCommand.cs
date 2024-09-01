using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Common.Interfaces.Services.ImageServices;
using ConnectVibe.Application.Common.Interfaces.Services.LocationServices;
using ConnectVibe.Domain.Common.Errors;
using ConnectVibe.Domain.Entities.Profile;
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Event.Commands.Create
{
    public class CreateEventDetailsCommand : IRequest<ErrorOr<EventDetailsResults>>
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string eventTitle { get; set; } = default!;
        public string eventDescription { get; set; } = default!;
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public TimeZoneInfo timeZone { get; set; } = default!;
        public Location Location { get; set; } = default!;
        public int capacity { get; set; }
        public string optional { get; set; } = default!;
    }
        
      


    public class CreateEventDetailCommandHandler : IRequestHandler<CreateEventDetailsCommand, ErrorOr<EventDetailsResults>>
    {
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IEventDetailsRepository _eventDetailsRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocationService _locationService;
        private readonly ILoggerManager _logger;

        public CreateEventDetailCommandHandler(IMapper mapper, IImageService imageService, IEventDetailsRepository eventDetailsRepository,
            ILocationService locationService, IUserRepository userRepository, ILoggerManager logger)
        {
            _mapper = mapper;
            _imageService = imageService;
            _eventDetailsRepository = eventDetailsRepository;
            _locationService = locationService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<EventDetailsResults>> Handle(CreateEventDetailsCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var user = _userRepository.GetUserByEmail(command.Email);
            _logger.LogInfo($"EventDetails command Result : {user}");
            if (user == null)
            {
                return Errors.Profile.ProfileNotFound;
            }
            var evetDetailss = _mapper.Map<CreateEventDetailsCommand>(command);
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

                evetDetailss.Location = location;
            }
            _eventDetailsRepository.Add(evetDetailss);
            var createdEvents = _mapper.Map<EventDetailsResults>(evetDetailss);

            return createdEvents;
        }
    }
}
