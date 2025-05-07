using ErrorOr;
using Fliq.Application.Authentication.Common.Profile;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Profile.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Fliq.Application.Profile.Commands.Update
{
    public class UpdateProfileCommand : IRequest<ErrorOr<CreateProfileResult>>
    {
        public int UserId { get; set; }
        public List<string>? Passions { get; set; } = new();
        public string? ProfileDescription { get; set; }
        public BusinessIdentificationDocumentMapped? BusinessIdentificationDocuments { get; set; }
        public List<ProfilePhotoMapped>? Photos { get; set; } = new();
        public List<ProfileType>? ProfileTypes { get; set; } = new();
        public DateTime? DOB { get; set; }
        public Gender? Gender { get; set; } = default!;
        public SexualOrientation? SexualOrientation { get; set; }
        public Religion? Religion { get; set; } = default!;
        public Ethnicity? Ethnicity { get; set; } = default!;
        public Occupation? Occupation { get; set; } = default!;
        public EducationStatus? EducationStatus { get; set; } = default!;
        public HaveKids? HaveKids { get; set; }
        public WantKids? WantKids { get; set; }
        public Location? Location { get; set; } = default!;
        public LocationDetail? LocationDetail { get; set; } = default!;
        public bool? AllowNotifications { get; set; }
    }

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ErrorOr<CreateProfileResult>>
    {
        private readonly IMapper _mapper;
        private readonly IMediaServices _mediaService;
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocationService _locationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerManager _loggerManager;
        private readonly IBusinessIdentificationDocumentRepository _businessIdentificationDocumentRepository;
        private readonly IDocumentUploadService _documentUploadService;
        private readonly IBusinessIdentificationDocumentTypeRepository _businessIdentificationDocumentTypeRepository;
        private const int UnauthorizedUserId = -1;

        public UpdateProfileCommandHandler(IMapper mapper, IMediaServices mediaService, IProfileRepository profileRepository, IUserRepository userRepository, ILocationService locationService, IHttpContextAccessor httpContextAccessor, ILoggerManager loggerManager, IBusinessIdentificationDocumentRepository businessIdentificationDocumentRepository, IDocumentUploadService documentUploadService, IBusinessIdentificationDocumentTypeRepository businessIdentificationDocumentTypeRepository)
        {
            _mapper = mapper;
            _mediaService = mediaService;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _locationService = locationService;
            _httpContextAccessor = httpContextAccessor;
            _loggerManager = loggerManager;
            _businessIdentificationDocumentRepository = businessIdentificationDocumentRepository;
            _documentUploadService = documentUploadService;
            _businessIdentificationDocumentTypeRepository = businessIdentificationDocumentTypeRepository;
        }

        public async Task<ErrorOr<CreateProfileResult>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _loggerManager.LogInfo($"Updating profile for user with id: {command.UserId}");

            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                _loggerManager.LogError("User not found");
                return Errors.User.UserNotFound;
            }
            var userProfile = _profileRepository.GetProfileByUserId(command.UserId);
            if (userProfile == null)
            {
                _loggerManager.LogError("User profile not found");
                return Errors.Profile.ProfileNotFound;
            }
            var updatedProfile = command.Adapt(userProfile);

            // Handle Photos Update
            if (command.Photos != null && command.Photos.Any())
            {
                userProfile.Photos.Clear();
                foreach (var photo in command.Photos)
                {
                    var profilePhotoUrl = await _mediaService.UploadImageAsync(photo.ImageFile);
                    if (profilePhotoUrl != null)
                    {
                        ProfilePhoto profilePhoto = new() { PictureUrl = profilePhotoUrl, Caption = photo.Caption };
                        updatedProfile.Photos.Add(profilePhoto);
                    }
                    else
                    {
                        _loggerManager.LogError("Failed to upload profile photo");
                        return Errors.Image.InvalidImage;
                    }
                }
            }

            //Handle BusinessIdentificationDocument Update

            if (command.BusinessIdentificationDocuments != null)
            {
                var documentTypeId = command.BusinessIdentificationDocuments.BusinessIdentificationDocumentTypeId;

                var documentTypeExists = await _businessIdentificationDocumentTypeRepository.DocumentTypeExists(documentTypeId);
                if (!documentTypeExists)
                {
                    _loggerManager.LogWarn($"Invalid DocumentTypeId: {documentTypeId}");
                    return Errors.Document.InvalidDocumentType;
                }

                if (command.BusinessIdentificationDocuments.BusinessIdentificationDocumentFront == null)
                {
                    _loggerManager.LogError("FrontPage is required.");
                    return Errors.Document.MissingFront;
                }

                var documentUploadResult = await _documentUploadService.UploadDocumentsAsync(
                    documentTypeId,
                    command.BusinessIdentificationDocuments.BusinessIdentificationDocumentFront,
                    command.BusinessIdentificationDocuments.BusinessIdentificationDocumentBack
                );

                if (!documentUploadResult.Success)
                {
                    _loggerManager.LogError($"Failed to upload business documents: {documentUploadResult.ErrorMessage}");
                    return Errors.Document.InvalidDocument;
                }

                updatedProfile.BusinessIdentificationDocument = new BusinessIdentificationDocument
                {
                    BusinessIdentificationDocumentTypeId = documentTypeId,
                    FrontDocumentUrl = documentUploadResult.FrontDocumentUrl,
                    BackDocumentUrl = documentUploadResult.BackDocumentUrl,
                    UploadedDate = DateTime.UtcNow,
                    IsVerified = false,
                };
            }


            // Handle Location Update
            if (command.Location != null)
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

                    updatedProfile.Location = location;
                }
            }

            updatedProfile.DateModified = DateTime.UtcNow;
            // Save updated profile
            _profileRepository.Update(updatedProfile);
            _loggerManager.LogInfo($"Profile updated successfully for user with id: {command.UserId}");

            return new CreateProfileResult(updatedProfile);
        }

        private int GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : UnauthorizedUserId;
        }
    }
}