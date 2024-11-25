﻿using ErrorOr;
using Fliq.Application.Authentication.Common.Profile;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Profile.Common;
using Fliq.Application.Prompts.Common.Helpers;
using Fliq.Contracts.Enums;
using Fliq.Contracts.Prompts;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Prompts;
using Fliq.Domain.Entities.Settings;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

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
        public List<PromptResponseDto> PromptResponses { get; set; } = new(); // for default prompt responses
        public bool AllowNotifications { get; set; }
        public int UserId { get; set; }
    }

    public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, ErrorOr<CreateProfileResult>>
    {
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocationService _locationService;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IPromptQuestionRepository _promptQuestionRepository;
        private readonly IPromptCategoryRepository _promptCategoryRepository;
        private readonly ILoggerManager _loggerManager;
        private readonly IMediaServices _mediaServices;


        public CreateProfileCommandHandler(IMapper mapper, IImageService imageService, IProfileRepository profileRepository, IUserRepository userRepository, ILocationService locationService, ISettingsRepository settingsRepository, ILoggerManager loggerManager, IPromptQuestionRepository promptQuestionRepository, IPromptCategoryRepository promptCategoryRepository, IMediaServices mediaServices)
        {
            _mapper = mapper;
            _imageService = imageService;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _locationService = locationService;
            _settingsRepository = settingsRepository;
            _loggerManager = loggerManager;
            _promptQuestionRepository = promptQuestionRepository;
            _promptCategoryRepository = promptCategoryRepository;
            _mediaServices = mediaServices;
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
                string? profileUrl = string.Empty;

                if (Debugger.IsAttached)
                {
                    profileUrl = await _mediaServices.UploadEventMediaAsync(photo.ImageFile);
                }
                else
                {
                    profileUrl = await _imageService.UploadImageAsync(photo.ImageFile);
                }
               
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

            // Process default prompt response
            var promptResponses = new List<PromptResponse>();
            foreach (var promptDto in command.PromptResponses)
            {
                var promptResponse = await ProcessPromptResponseAsync(promptDto, userProfile);
                if (promptResponse.IsError)
                    return promptResponse.Errors;
                promptResponses.Add(promptResponse.Value);
            }


            _profileRepository.Add(userProfile);

            Setting setting = new()
            {
                UserId = command.UserId,
            };
            _settingsRepository.Add(setting);

            return new CreateProfileResult(userProfile);
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
                    return Errors.Prompts.QuestionNotFound; // Return null or handle as appropriate if the question is invalid
                }
            }

            // Set up the prompt response entity
            var promptResponse = new PromptResponse
            {
                PromptQuestion = promptQuestion,
                UserProfile = userProfile,
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

        private async Task<string?> UploadPromptAnswerAsync(IFormFile file, PromptAnswerMediaType type)
        {
            // Determine the container name or local folder path based on media type
            string? containerName = type switch
            {
                PromptAnswerMediaType.VoiceNote => "audio-prompts",
                PromptAnswerMediaType.VideoClip => "video-prompts",
                _ => null
            } ?? throw new ArgumentException("Invalid prompt answer type provided.");

            // Check if the app is in Debug Mode
            if (Debugger.IsAttached){
                // In Debug mode, save the file to a local directory instead of uploading to the server
                return await _mediaServices.UploadEventMediaAsync(file);
            }
            else
            {
                //In Release mode, upload the file to the server
                _loggerManager.LogDebug($"Uploading file to container: {containerName}");
                var uploadResult = await _imageService.UploadMediaAsync(file, containerName);
                return uploadResult; // Return the URL or path from server upload
            }
   
        }

    }
}