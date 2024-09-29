using ErrorOr;
using Fliq.Application.Authentication.Common.Profile;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Profile.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Settings;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Fliq.Application.Profile.Commands.Create
{
    public class CreateProfileCommand : IRequest<ErrorOr<CreateProfileResult>>
    {
        public List<string> Passions { get; set; } = new();
        public string? ProfileDescription { get; set; }
        public List<ProfilePhotoMapped> Photos { get; set; } = new();
        public List<ProfileType> ProfileTypes { get; set; } = new();
        public DateTime DOB { get; set; }
        public Gender Gender { get; set; } = default!;
        public SexualOrientation? SexualOrientation { get; set; }
        public Religion Religion { get; set; } = default!;
        public Ethnicity Ethnicity { get; set; } = default!;
        public Occupation Occupation { get; set; } = default!;
        public EducationStatus EducationStatus { get; set; } = default!;
        public HaveKids? HaveKids { get; set; }
        public WantKids? WantKids { get; set; }
        public Location Location { get; set; } = default!;
        public LocationDetail LocationDetail { get; set; } = default!;
        public bool AllowNotifications { get; set; }
    }

    public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, ErrorOr<CreateProfileResult>>
    {
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocationService _locationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingsRepository _settingsRepository;
        private const int UnauthorizedUserId = -1;

        public CreateProfileCommandHandler(IMapper mapper, IImageService imageService, IProfileRepository profileRepository, IUserRepository userRepository, ILocationService locationService, IHttpContextAccessor httpContextAccessor, ISettingsRepository settingsRepository)
        {
            _mapper = mapper;
            _imageService = imageService;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _locationService = locationService;
            _httpContextAccessor = httpContextAccessor;
            _settingsRepository = settingsRepository;
        }

        public async Task<ErrorOr<CreateProfileResult>> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var userId = GetUserId();

            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return Errors.Profile.ProfileNotFound;
            }
            var existingProfile = _profileRepository.GetUserProfileByUserId(userId);
            if (existingProfile != null)
            {
                return Errors.Profile.DuplicateProfile;
            }

            var userProfile = _mapper.Map<UserProfile>(command);
            userProfile.UserId = 0;
            userProfile.Photos = new();
            userProfile.User = user;
            foreach (var photo in command.Photos)
            {
                var profileUrl = await _imageService.UploadMediaAsync(photo.ImageFile);
                if (profileUrl != null)
                {
                    ProfilePhoto profilePhoto = new() { PictureUrl = profileUrl, Caption = photo.Caption };
                    userProfile.Photos.Add(profilePhoto);
                }
                else
                {
                    return Errors.Image.InvalidImage;
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

                userProfile.Location = location;
            }

            _profileRepository.Add(userProfile);

            Setting setting = new()
            {
                UserId = userId
            };
            _settingsRepository.Add(setting);

            return new CreateProfileResult(userProfile);
        }

        private int GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : UnauthorizedUserId;
        }
    }
}