﻿using ConnectVibe.Application.Authentication.Common.Profile;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services.ImageServices;
using ConnectVibe.Application.Common.Interfaces.Services.LocationServices;
using ConnectVibe.Application.Profile.Common;
using ConnectVibe.Domain.Common.Errors;
using ConnectVibe.Domain.Entities.Profile;
using ErrorOr;
using MapsterMapper;
using MediatR;

namespace ConnectVibe.Application.Profile.Commands.Create
{
    public class CreateProfileCommand : IRequest<ErrorOr<CreateProfileResult>>
    {
        public int UserId { get; set; }
        public List<string> Passions { get; set; } = default!;
        public List<ProfilePhotoMapped> Photos { get; set; } = default!;
        public DateTime DOB { get; set; }
        public Gender Gender { get; set; } = default!;
        public SexualOrientation SexualOrientation { get; set; } = default!;
        public Religion Religion { get; set; } = default!;
        public Ethnicity Ethnicity { get; set; } = default!;
        public HaveKids HaveKids { get; set; } = default!;
        public WantKids WantKids { get; set; } = default!;
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

        public CreateProfileCommandHandler(IMapper mapper, IImageService imageService, IProfileRepository profileRepository, IUserRepository userRepository, ILocationService locationService)
        {
            _mapper = mapper;
            _imageService = imageService;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _locationService = locationService;
        }

        public async Task<ErrorOr<CreateProfileResult>> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                return Errors.Profile.ProfileNotFound;
            }
            var existingProfile = _profileRepository.GetUserProfileByUserId(command.UserId);
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

            return new CreateProfileResult(userProfile);
        }
    }
}