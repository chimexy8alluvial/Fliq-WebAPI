using ConnectVibe.Application.Authentication.Common.Profile;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services.ImageServices;
using ConnectVibe.Contracts.Profile;
using ConnectVibe.Domain.Common.Errors;
using ConnectVibe.Domain.Entities.Profile;
using ErrorOr;
using MapsterMapper;
using MediatR;

namespace ConnectVibe.Application.Profile.Commands.Create
{
    public record CreateProfileCommand(
        int UserId,
         List<string> Passions,
    List<ProfilePhotoDto> Photos,
           DateTime DOB,
     Gender Gender,
     SexualOrientation SexualOrientation,
     Religion Religion,
     Ethnicity Ethnicity,
     HaveKids HaveKids,
     WantKids WantKids,
     bool ShareLocation = default!,
     bool AllowNotifications = false
    
        ) : IRequest<ErrorOr<CreateProfileResult>>;

    public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, ErrorOr<CreateProfileResult>>
    {
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IProfileRepository _profileRepository;

        public CreateProfileCommandHandler(IMapper mapper, IImageService imageService, IProfileRepository profileRepository, IUserRepository userRepository)
        {
            _mapper = mapper;
            _imageService = imageService;
            _profileRepository = profileRepository;
        }

        public async Task<ErrorOr<CreateProfileResult>> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var existingProfile = _profileRepository.GetUserProfileByUserId(command.UserId);
            if (existingProfile != null)
            {
                return Errors.Profile.DuplicateProfile;
            }

            var userProfile = _mapper.Map<UserProfile>(command);

            foreach (var photo in command.Photos)
            {
                var profileUrl = await _imageService.UploadImageAsync(photo.ImageFile);
                if (profileUrl != null)
                {
                    ProfilePhoto profilePhoto = new() { PictureUrl = profileUrl, Caption = photo.Caption };
                    userProfile.Photos.Add(profilePhoto);
                }
            }
            _profileRepository.Add(userProfile);

            return new CreateProfileResult(userProfile);
        }
    }
}