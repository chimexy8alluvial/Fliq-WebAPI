using ErrorOr;
using Fliq.Application.Authentication.Common.Profile;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Payments.Commands.CreateWallet;
using Fliq.Application.Profile.Common;
using Fliq.Application.Prompts.Common.Helpers;
using Fliq.Contracts.Prompts;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Prompts;
using Fliq.Domain.Enums;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Profile.Commands.Create
{
    public class CreateProfileCommand : IRequest<ErrorOr<CreateProfileResult>>
    {
        public int UserId { get; set; }
        public int? SexualOrientationId { get; set; }
        public bool IsSexualOrientationVisible { get; set; }
        public int? EducationStatusId { get; set; }
        public bool IsEducationStatusVisible { get; set; }
        public int? EthnicityId { get; set; }
        public bool IsEthnicityVisible { get; set; }
        public int? OccupationId { get; set; }
        public bool IsOccupationVisible { get; set; }
        public int? ReligionId { get; set; }
        public bool IsReligionVisible { get; set; }
        public bool AllowNotifications { get; set; }
        public DateTime? DOB { get; set; }
        public string? ProfileDescription { get; set; }
        public ProfileSection CurrentSection { get; set; }
        public int GenderId { get; set; }
        public int HaveKidsId { get; set; }
        public int WantKidsId { get; set; }
        public Location? Location { get; set; }
        public LocationDetail? LocationDetail { get; set; }
        public List<PromptResponseDto>? PromptResponses { get; set; } = new(); // for default prompt responses
        public List<string>? Passions { get; set; }
        public BusinessIdentificationDocumentMapped? BusinessIdentificationDocuments {  get; set; } 
        public List<ProfilePhotoMapped>? Photos { get; set; }
        public List<ProfileType>? ProfileTypes { get; set; }
    }

    public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, ErrorOr<CreateProfileResult>>
    {
        private readonly IMapper _mapper;
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocationService _locationService;
        private readonly IPromptQuestionRepository _promptQuestionRepository;
        private readonly IPromptCategoryRepository _promptCategoryRepository;
        private readonly ILoggerManager _loggerManager;
        private readonly IMediaServices _mediaServices;
        private readonly IDocumentUploadService _documentUploadService;
        private readonly IBusinessIdentificationDocumentRepository _businessIdentificationDocumentRepository;
        private readonly IBusinessIdentificationDocumentTypeRepository _businessIdentificationDocumentTypeRepository;
        private readonly IMediator _mediator;

        public CreateProfileCommandHandler(IMapper mapper, IProfileRepository profileRepository, IUserRepository userRepository, ILocationService locationService, ILoggerManager loggerManager, IPromptQuestionRepository promptQuestionRepository, IPromptCategoryRepository promptCategoryRepository, IMediaServices mediaServices, IDocumentUploadService documentUploadService, IBusinessIdentificationDocumentRepository businessIdentificationDocumentRepository, IBusinessIdentificationDocumentTypeRepository businessIdentificationDocumentTypeRepository, IMediator mediator)
        {
            _mapper = mapper;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _locationService = locationService;
            _loggerManager = loggerManager;
            _promptQuestionRepository = promptQuestionRepository;
            _promptCategoryRepository = promptCategoryRepository;
            _mediaServices = mediaServices;
            _documentUploadService = documentUploadService;
            _businessIdentificationDocumentRepository = businessIdentificationDocumentRepository;
            _businessIdentificationDocumentTypeRepository = businessIdentificationDocumentTypeRepository;
            _mediator = mediator;
        }

        public async Task<ErrorOr<CreateProfileResult>> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
        {
            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                _loggerManager.LogError("User not found. Aborting profile creation.");
                return Errors.Profile.ProfileNotFound;
            }

            var existingProfile = _profileRepository.GetProfileByUserId(command.UserId);
            if (existingProfile != null)
            {
                _loggerManager.LogWarn($"Profile already exists for UserId: {command.UserId}");
                return Errors.Profile.DuplicateProfile;
            }

            if(user.Wallet == null)
            {
                // Create wallet for the user 
                _loggerManager.LogInfo($"Creating wallet for user {command.UserId} after profile creation");
                var createWalletCommand = new CreateWalletCommand(command.UserId);
                var walletResult = await _mediator.Send(createWalletCommand, cancellationToken);
            }

            existingProfile = new UserProfile
            {
                UserId = command.UserId,
                User = user,
                GenderId = command.GenderId,
                WantKidsId = command.WantKidsId,
                HaveKidsId = command.HaveKidsId,
                Photos = new List<ProfilePhoto>(), // Initialize Photos
                PromptResponses = new List<PromptResponse>(), // Initialize PromptResponses
                CompletedSections = new List<string>() // Initialize CompletedSections
            };
            _profileRepository.Add(existingProfile);

            if (!existingProfile.CompletedSections.Contains(command.CurrentSection.ToString()))
            {
                existingProfile.CompletedSections.Add(command.CurrentSection.ToString());
            }

            existingProfile = _mapper.Map(command, existingProfile); // Use Mapster's Map instead of Adapt for clarity

            if (command.Photos?.Any() == true)
            {
                foreach (var photo in command.Photos)
                {
                    var profileUrl = await _mediaServices.UploadImageAsync(photo.ImageFile);
                    if (profileUrl != null)
                    {
                        existingProfile.Photos.Add(new ProfilePhoto { PictureUrl = profileUrl, Caption = photo.Caption });
                    }
                    else
                    {
                        _loggerManager.LogError("Failed to upload image. Aborting profile creation.");
                        return Errors.Image.InvalidImage;
                    }
                }
            }

            if (command.BusinessIdentificationDocuments != null)
            {
                var documentTypeId = command.BusinessIdentificationDocuments.BusinessIdentificationDocumentTypeId;
                var documentType = await _businessIdentificationDocumentTypeRepository.GetByIdAsync(documentTypeId);
                if (documentType == null || documentType.IsDeleted)
                {
                    _loggerManager.LogWarn($"Invalid DocumentTypeId: {documentTypeId}");
                    return Errors.Document.InvalidDocumentType;
                }

                var documentUploadResult = await _documentUploadService.UploadDocumentsAsync(
                    documentTypeId,
                    command.BusinessIdentificationDocuments.BusinessIdentificationDocumentFront,
                    command.BusinessIdentificationDocuments.BusinessIdentificationDocumentBack);

                if (!documentUploadResult.Success)
                {
                    _loggerManager.LogError($"Failed to upload business documents: {documentUploadResult.ErrorMessage}");
                    return Errors.Document.InvalidDocument;
                }

                var businessIdentificationDocument = new BusinessIdentificationDocument
                {
                    BusinessIdentificationDocumentTypeId = documentTypeId,
                    FrontDocumentUrl = documentUploadResult.FrontDocumentUrl,
                    BackDocumentUrl = documentUploadResult.BackDocumentUrl,
                    IsVerified = false,
                };

                existingProfile.BusinessIdentificationDocument = businessIdentificationDocument;
                _businessIdentificationDocumentRepository.Add(businessIdentificationDocument);
            }

            if (command.Location != null)
            {
                var locationResponse = await _locationService.GetAddressFromCoordinatesAsync(command.Location.Lat, command.Location.Lng);
                if (locationResponse == null)
                {
                    _loggerManager.LogError("Location service failed to return a valid response.");
                    return Errors.Profile.InvalidPayload; // More specific error
                }

                LocationDetail locationDetail = _mapper.Map<LocationDetail>(locationResponse);
                Location location = new Location
                {
                    LocationDetail = locationDetail,
                    IsVisible = command.Location.IsVisible,
                    Lat = command.Location.Lat,
                    Lng = command.Location.Lng
                };

                existingProfile.Location = location;
            }

            if (command.PromptResponses?.Any() == true)
            {
                foreach (var promptDto in command.PromptResponses)
                {
                    var promptResponse = await ProcessPromptResponseAsync(promptDto, existingProfile);
                    if (promptResponse.IsError)
                        return promptResponse.Errors;

                    existingProfile.PromptResponses.Add(promptResponse.Value);
                }
            }

            existingProfile.CompletedSections = existingProfile.CompletedSections.Distinct().ToList();
            _profileRepository.Update(existingProfile);

            return new CreateProfileResult(existingProfile);
        }

        private async Task<ErrorOr<PromptResponse>> ProcessPromptResponseAsync(PromptResponseDto promptDto, UserProfile userProfile)
        {
            if (string.IsNullOrWhiteSpace(promptDto.TextResponse) && promptDto.VoiceNote == null && promptDto.VideoClip == null)
            {
                _loggerManager.LogWarn("No answer format provided. At least one format (Text, Voice, or Video) must be supplied. Aborting answer creation.");
                return Errors.Prompts.AnswerNotProvided;
            }

            var category = _promptCategoryRepository.GetCategoryById(promptDto.CategoryId);
            if (category == null)
            {
                _loggerManager.LogWarn($"Invalid CategoryId: {promptDto.CategoryId}");
                return Errors.Prompts.CategoryNotFound;
            }

            PromptQuestion? promptQuestion;
            if (promptDto.IsCustomPrompt)
            {
                if (string.IsNullOrWhiteSpace(promptDto.CustomPromptQuestionText))
                {
                    _loggerManager.LogWarn("Custom prompt question text is missing.");
                    return Errors.Prompts.QuestionNotFound;
                }

                promptQuestion = new PromptQuestion
                {
                    QuestionText = promptDto.CustomPromptQuestionText,
                    IsSystemGenerated = false,
                    PromptCategoryId = promptDto.CategoryId,
                    CustomPromptId = PromptIdHelper.GenerateCustomPromptId(userProfile.UserId, category.CategoryName)
                };
                _promptQuestionRepository.AddQuestion(promptQuestion);
            }
            else
            {
                promptQuestion =  _promptQuestionRepository.GetQuestionByIdAsync(promptDto.PromptQuestionId);
                if (promptQuestion == null)
                {
                    _loggerManager.LogWarn($"Invalid PromptQuestionId: {promptDto.PromptQuestionId}");
                    return Errors.Prompts.QuestionNotFound;
                }
            }

            var promptResponse = new PromptResponse
            {
                PromptQuestionId = promptQuestion.Id,
                UserProfileId = userProfile.Id,
                ResponseType = promptDto.TextResponse != null ? nameof(PromptAnswerMediaType.Text) :
                               promptDto.VideoClip != null ? nameof(PromptAnswerMediaType.VideoClip) :
                               nameof(PromptAnswerMediaType.VoiceNote)
            };

            if (promptDto.TextResponse != null)
                promptResponse.Response = promptDto.TextResponse;

            if (promptDto.VoiceNote != null)
                promptResponse.Response = await UploadPromptAnswerAsync(promptDto.VoiceNote, PromptAnswerMediaType.VoiceNote);

            if (promptDto.VideoClip != null)
                promptResponse.Response = await UploadPromptAnswerAsync(promptDto.VideoClip, PromptAnswerMediaType.VideoClip);

            return promptResponse;
        }

        private async Task<string> UploadPromptAnswerAsync(IFormFile file, PromptAnswerMediaType type)
        {
            string containerName = type switch
            {
                PromptAnswerMediaType.VoiceNote => "audio-prompts",
                PromptAnswerMediaType.VideoClip => "video-prompts",
                _ => throw new ArgumentException("Invalid prompt answer type provided.")
            };

            _loggerManager.LogDebug($"Uploading file to container: {containerName}");
            var uploadResult = await _mediaServices.UploadMediaAsync(file, containerName);
            if (uploadResult == null)
                throw new ArgumentException("Failed to get response url.");
            return uploadResult;
        }
    }
}