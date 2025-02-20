using ErrorOr;
using Fliq.Application.Authentication.Common.Profile;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
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
        public List<string>? Passions { get; set; }
        public string? ProfileDescription { get; set; }
        public List<ProfilePhotoMapped>? Photos { get; set; }
        public List<ProfileType>? ProfileTypes { get; set; }
        public ProfileSection CurrentSection { get; set; }
        public DateTime? DOB { get; set; }
        public Gender? Gender { get; set; }
        public SexualOrientation? SexualOrientation { get; set; }
        public Religion? Religion { get; set; }
        public Ethnicity? Ethnicity { get; set; }
        public Occupation? Occupation { get; set; }
        public EducationStatus? EducationStatus { get; set; }
        public HaveKids? HaveKids { get; set; }
        public WantKids? WantKids { get; set; }
        public Location? Location { get; set; }
        public LocationDetail? LocationDetail { get; set; }
        public List<PromptResponseDto>? PromptResponses { get; set; } = new(); // for default prompt responses
        public bool AllowNotifications { get; set; }
        public int UserId { get; set; }
    }

    public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, ErrorOr<CreateProfileResult>>
    {
        private readonly IMapper _mapper;
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocationService _locationService;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IPromptQuestionRepository _promptQuestionRepository;
        private readonly IPromptCategoryRepository _promptCategoryRepository;
        private readonly ILoggerManager _loggerManager;
        private readonly IMediaServices _mediaServices;
        private readonly IPromptResponseRepository _promptResponseRepository;

        public CreateProfileCommandHandler(IMapper mapper, IProfileRepository profileRepository, IUserRepository userRepository, ILocationService locationService, ISettingsRepository settingsRepository, ILoggerManager loggerManager, IPromptQuestionRepository promptQuestionRepository, IPromptCategoryRepository promptCategoryRepository, IMediaServices mediaServices, IPromptResponseRepository promptResponseRepository)
        {
            _mapper = mapper;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _locationService = locationService;
            _settingsRepository = settingsRepository;
            _loggerManager = loggerManager;
            _promptQuestionRepository = promptQuestionRepository;
            _promptCategoryRepository = promptCategoryRepository;
            _mediaServices = mediaServices;
            _promptResponseRepository = promptResponseRepository;
        }

        public async Task<ErrorOr<CreateProfileResult>> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                return Errors.Profile.ProfileNotFound;
            }

            var existingProfile = _profileRepository.GetProfileByUserId(command.UserId);
            if (existingProfile == null)
            {
                // Create a new profile if none exists
                existingProfile = new UserProfile { UserId = command.UserId, User = user };
                _profileRepository.Add(existingProfile);
            }

            // Track session completeness using an enum
            if (!existingProfile.CompletedSections.Contains(command.CurrentSection.ToString()))
            {
                existingProfile.CompletedSections.Add(command.CurrentSection.ToString());
            }

            // Map non-null properties using Mapster Adapt
            existingProfile = command.Adapt(existingProfile);

            if (command.Photos is not null)
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
                        return Errors.Image.InvalidImage;
                    }
                }
            }

            if (command.Location != null)
            {
                var locationResponse = await _locationService.GetAddressFromCoordinatesAsync(command.Location.Lat, command.Location.Lng);
                if (locationResponse is not null)
                {
                    existingProfile.Location = locationResponse.Adapt<Location>();
                }
            }

            if (command.PromptResponses.Any())
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
            //Validate answer was provided
            if (string.IsNullOrWhiteSpace(promptDto.TextResponse) && promptDto.VoiceNote == null && promptDto.VideoClip == null)
            {
                _loggerManager.LogWarn("No answer format provided. At least one format (Text, Voice, or Video) must be supplied. Aborting answer creation.");
                return Errors.Prompts.AnswerNotProvided;
            }
            // Validate the CategoryId
            var category = _promptCategoryRepository.GetCategoryById(promptDto.CategoryId);
            if (category == null)
            {
                return Errors.Prompts.CategoryNotFound; // Return null or handle as appropriate if the category is invalid
            }

            PromptQuestion? promptQuestion;

            if (promptDto.IsCustomPrompt)
            {
                if (string.IsNullOrWhiteSpace(promptDto.CustomPromptQuestionText)) return Errors.Prompts.QuestionNotFound;
                // Create a new custom prompt question if it doesn't exist
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
                // Retrieve an existing system prompt question
                promptQuestion = _promptQuestionRepository.GetQuestionByIdAsync(promptDto.PromptQuestionId);
                if (promptQuestion == null)
                {
                    return Errors.Prompts.QuestionNotFound;
                }
            }

            // Set up the prompt response entity
            var promptResponse = new PromptResponse
            {
                PromptQuestionId = promptQuestion.Id,
                UserProfileId = userProfile.Id,
                ResponseType = promptDto.TextResponse != null ? nameof(PromptAnswerMediaType.Text) :
                               promptDto.VideoClip != null ? nameof(PromptAnswerMediaType.VideoClip) :
                               nameof(PromptAnswerMediaType.VoiceNote)
            };

            // Process responses and set the appropriate URLs
            if (promptDto.TextResponse is not null)
                promptResponse.Response = promptDto.TextResponse;

            if (promptDto.VoiceNote is not null)
                promptResponse.Response = await UploadPromptAnswerAsync(promptDto.VoiceNote, PromptAnswerMediaType.VoiceNote);

            if (promptDto.VideoClip is not null)
                promptResponse.Response = await UploadPromptAnswerAsync(promptDto.VideoClip, PromptAnswerMediaType.VideoClip);

            return promptResponse;
        }

        private async Task<string> UploadPromptAnswerAsync(IFormFile file, PromptAnswerMediaType type)
        {
            // Determine the container name or local folder path based on media type
            string? containerName = type switch
            {
                PromptAnswerMediaType.VoiceNote => "audio-prompts",
                PromptAnswerMediaType.VideoClip => "video-prompts",
                _ => null
            } ?? throw new ArgumentException("Invalid prompt answer type provided.");

            //upload the file to the server
            _loggerManager.LogDebug($"Uploading file to container: {containerName}");
            var uploadResult = await _mediaServices.UploadMediaAsync(file, containerName) ?? throw new ArgumentException("Failed to get response url.");
            return uploadResult; // Return the URL or path from server upload
        }
    }
}