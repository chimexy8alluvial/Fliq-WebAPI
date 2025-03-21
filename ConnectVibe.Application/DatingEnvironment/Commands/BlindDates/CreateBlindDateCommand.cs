using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.DatingEnvironment.Common;
using Fliq.Application.DatingEnvironment.Common.BlindDates;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.DatingEnvironment;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.Profile;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.DatingEnvironment.Commands.BlindDates
{
    public record CreateBlindDateCommand(
       int CreatedByUserId,
       int CategoryId,
       string Title,
       DateTime StartDateTime,
       bool IsOneOnOne,
       int? NumberOfParticipants,
       bool IsRecordingEnabled,
       DateTime? SessionStartTime,
       DateTime? SessionEndTime,
       DatePhotoMapped? BlindDateImage,
       Location Location,
       LocationDetail LocationDetail
        ) : IRequest<ErrorOr<CreateBlindDateResult>>;

    public class AddBlindDateCommandHandler : IRequestHandler<CreateBlindDateCommand, ErrorOr<CreateBlindDateResult>>
    {
        private readonly IBlindDateRepository _blindDateRepository;
        private readonly IBlindDateCategoryRepository _blindDateCategoryRepository;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private readonly ILocationService _locationService;
        private readonly IMediaServices _mediaServices;
        private readonly IBlindDateParticipantRepository _blindDateParticipantRepository;

        public AddBlindDateCommandHandler(IBlindDateRepository blindDateRepository, ILoggerManager loggerManager, IBlindDateCategoryRepository blindDateCategoryRepository, IMapper mapper, ILocationService locationService, IMediaServices mediaServices, IBlindDateParticipantRepository blindDateParticipantRepository)
        {
            _blindDateRepository = blindDateRepository;
            _loggerManager = loggerManager;
            _blindDateCategoryRepository = blindDateCategoryRepository;
            _mapper = mapper;
            _locationService = locationService;
            _mediaServices = mediaServices;
            _blindDateParticipantRepository = blindDateParticipantRepository;
        }

        public async Task<ErrorOr<CreateBlindDateResult>> Handle(CreateBlindDateCommand command, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"Attempting to create blind date: {command.Title}");

            var category = await _blindDateCategoryRepository.GetByIdAsync(command.CategoryId);
            if (category == null)
            {
                _loggerManager.LogWarn($"No blind date category found for categoryId: {command.CategoryId}. Aborting creation.");
                return Errors.Dating.BlindDateCategoryNotFound;
            }

            // Map the BlindDate entity
            var blindDate = _mapper.Map<BlindDate>(command);

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
                blindDate.Location = location;
            }
            else
            {
                _loggerManager.LogWarn("Failed to retrieve location details from coordinates.");
            }

            // Upload blind date image
            if (command.BlindDateImage != null)
            {
                blindDate.ImageUrl = await _mediaServices.UploadImageAsync(command.BlindDateImage.BlindDateSessionImageFile);
            }

            blindDate.NumberOfParticipants = command.IsOneOnOne ? 1 : command.NumberOfParticipants
                ?? throw new ArgumentException("NumberOfParticipants is required for multiple sessions.");

            //save the blind date
            await _blindDateRepository.AddAsync(blindDate);

            // add the creator as a participant
            var creatorParticipant = new BlindDateParticipant
            {
                BlindDateId = blindDate.Id,
                UserId = command.CreatedByUserId,
                IsCreator = true
            };

            await _blindDateParticipantRepository.AddAsync(creatorParticipant);

            _loggerManager.LogInfo($"Successfully created Blind Date: {blindDate.Title} with ID: {blindDate.Id}");

            return new CreateBlindDateResult(blindDate.Id, blindDate.Title);
        }

    }
}
