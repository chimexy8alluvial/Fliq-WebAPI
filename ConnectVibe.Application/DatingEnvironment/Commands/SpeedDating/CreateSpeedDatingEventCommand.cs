using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Common.SpeedDating;
using Fliq.Contracts.Dating;
using Fliq.Contracts.Profile;
using Fliq.Domain.Entities.Profile;
using MapsterMapper;
using MediatR;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Infrastructure.Persistence.Repositories;

namespace Fliq.Application.DatingEnvironment.Commands.SpeedDating
{
    public record CreateSpeedDatingEventCommand(
        int CreatedByUserId,
         string Title,
        string Category,
        DateTime StartTime,
        int MinAge,
        int MaxAge,
        int MaxParticipants,
        int DurationPerPairingMinutes,
        BlindDatePhotoDto? SpeedDateImage,
        LocationDto Location,
        LocationDetail LocationDetail
        ) : IRequest<ErrorOr<CreateSpeedDatingEventResult>>;

    public class CreateSpeedDatingEventCommandHandler : IRequestHandler<CreateSpeedDatingEventCommand, ErrorOr<CreateSpeedDatingEventResult>>
    {
        private readonly ISpeedDatingEventRepository _speedDateRepository;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private readonly ILocationService _locationService;
        private readonly IMediaServices _mediaServices;
        private readonly ISpeedDateParticipantRepository _speedDateParticipantRepository;

        public CreateSpeedDatingEventCommandHandler(ISpeedDatingEventRepository speedDateRepository, ILoggerManager loggerManager, IMapper mapper, ILocationService locationService, IMediaServices mediaServices, ISpeedDateParticipantRepository speedDateParticipantRepository)
        {
            _speedDateRepository = speedDateRepository;
            _loggerManager = loggerManager;
            _mapper = mapper;
            _locationService = locationService;
            _mediaServices = mediaServices;
            _speedDateParticipantRepository = speedDateParticipantRepository;
        }

        public async Task<ErrorOr<CreateSpeedDatingEventResult>> Handle(CreateSpeedDatingEventCommand command, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"Attempting to create a speed dating event: {command.Title}");


            // Map the BlindDate entity
            var speedDate = _mapper.Map<SpeedDatingEvent>(command);

            // Get enriched location details
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
                speedDate.Location = location;
            }
            else
            {
                _loggerManager.LogWarn("Failed to retrieve location details from coordinates.");
            }

            // Upload blind date image
            if (command.SpeedDateImage != null)
            {
                speedDate.ImageUrl = await _mediaServices.UploadImageAsync(command.SpeedDateImage.BlindDateSessionImageFile);
            }



            //save the blind date
            await _speedDateRepository.AddAsync(speedDate);

            // add the creator as a participant
            var creatorParticipant = new SpeedDatingParticipant
            {
                SpeedDatingEventId = speedDate.Id,
                UserId = command.CreatedByUserId,
                IsCreator = true
            };

            await _speedDateParticipantRepository.AddAsync(creatorParticipant);

            _loggerManager.LogInfo($"Successfully created Speed Date Event: {speedDate.Title} with ID: {speedDate.Id}");

            return new CreateSpeedDatingEventResult(speedDate.Id, speedDate.Title);
        }

    }
}
