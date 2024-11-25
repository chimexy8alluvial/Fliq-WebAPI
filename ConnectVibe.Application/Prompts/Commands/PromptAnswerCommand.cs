using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.Prompts.Common;
using Fliq.Contracts.Enums;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Prompts;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Prompts.Commands
{
    public record PromptAnswerCommand(
        int UserId,
        int PromptQuestionId,
        string? CustomPromptId = null, // Optional for standard prompts
        string? TextAnswer = null,
        IFormFile? VoiceNote = null,
        IFormFile? VideoClip = null
        ) : IRequest<ErrorOr<CreatePromptAnswerResult>>;


    public class PromptAnswerCommandHandler : IRequestHandler<PromptAnswerCommand, ErrorOr<CreatePromptAnswerResult>>
    {
        private readonly IImageService _mediaService;
        private readonly IPromptAnswerRepository _promptAnswerRepository;
        private readonly IPromptQuestionRepository _promptQuestionRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _loggerManager;

        public PromptAnswerCommandHandler(IImageService mediaService, IPromptAnswerRepository promptAnswerRepository, IPromptQuestionRepository promptQuestionRepository, IMapper mapper, ILoggerManager loggerManager)
        {
            _mediaService = mediaService;
            _promptAnswerRepository = promptAnswerRepository;
            _promptQuestionRepository = promptQuestionRepository;
            _mapper = mapper;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<CreatePromptAnswerResult>> Handle(PromptAnswerCommand request, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"Starting answer creation process for Prompt Question ID: {request.PromptQuestionId} by User ID: {request.UserId}");

            if (string.IsNullOrWhiteSpace(request.TextAnswer) && request.VoiceNote == null && request.VideoClip == null)
            {
                _loggerManager.LogWarn("No answer format provided. At least one format (Text, Voice, or Video) must be supplied. Aborting answer creation.");
                return Errors.Prompts.AnswerNotProvided;
            }

            var promptQuestion = _promptQuestionRepository.GetQuestionByIdAsync(request.PromptQuestionId);
            if(promptQuestion is null)
            {
                _loggerManager.LogWarn($"Prompt Question not found for ID: {request.PromptQuestionId}. Aborting answer creation.");
                return Errors.Prompts.QuestionNotFound;
            }

            _loggerManager.LogDebug("Mapping request data to PromptAnswer model.");
            var promptAnswer = _mapper.Map<PromptAnswer>(request);

            if (request.VoiceNote != null)
            {
                _loggerManager.LogInfo("Uploading voice note for Prompt Answer.");
                promptAnswer.VoiceNoteUrl = await UploadPromptAnswerAsync(request.VoiceNote, PromptAnswerMediaType.VoiceNote);
                _loggerManager.LogDebug($"Voice note uploaded successfully. URL: {promptAnswer.VideoClipUrl}");
            }

            if (request.VideoClip != null)
            {
                promptAnswer.VideoClipUrl = await UploadPromptAnswerAsync(request.VideoClip, PromptAnswerMediaType.VideoClip);
            }

             _promptAnswerRepository.Add(promptAnswer);
            _loggerManager.LogInfo($"Prompt Answer added successfully for Prompt Question ID: {request.PromptQuestionId} with Answer ID: {promptAnswer.Id}");

            return new CreatePromptAnswerResult(request.PromptQuestionId, promptAnswer.Id, true);
        }

        private async Task<string?> UploadPromptAnswerAsync(IFormFile file, PromptAnswerMediaType type)
        {
            string? containerName = type switch
            {
                PromptAnswerMediaType.VoiceNote => "audio-prompts",
                PromptAnswerMediaType.VideoClip => "video-prompts",
                _ => null
            } ?? throw new ArgumentException("Invalid prompt answer type provided.");

            _loggerManager.LogDebug($"Uploading file to container: {containerName}");
            var uploadResult = await _mediaService.UploadMediaAsync(file, containerName);
            _loggerManager.LogDebug($"File uploaded to {containerName} with result URL: {uploadResult}");

            return uploadResult;
        }

    }


}
