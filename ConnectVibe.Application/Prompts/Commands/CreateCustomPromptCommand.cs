using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.Prompts.Common;
using Fliq.Application.Prompts.Common.Helpers;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Prompts;
using Fliq.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;



namespace Fliq.Application.Prompts.Commands
{
    public record CreateCustomPromptCommand(
        int UserId,
        string QuestionText,
        int PromptCategoryId,
        PromptAnswerMediaType MediaType,
        string? TextAnswer = null,
        IFormFile? VoiceNote = null,
        IFormFile? VideoClip = null
        ) : IRequest<ErrorOr<CreatePromptAnswerResult>>;

    public class CreateCustomPromptCommandHandler : IRequestHandler<CreateCustomPromptCommand, ErrorOr<CreatePromptAnswerResult>>
    {
        private readonly IPromptQuestionRepository _promptQuestionRepository;
        private readonly IImageService _mediaService;
        private readonly IPromptCategoryRepository _promptCategoryRepository;
        private readonly ISender _mediator;
        private readonly ILoggerManager _loggerManager;

        public CreateCustomPromptCommandHandler(IPromptQuestionRepository promptQuestionRepository, IImageService mediaService, IPromptCategoryRepository promptCategoryRepository, ISender mediator, ILoggerManager loggerManager)
        {
            _promptQuestionRepository = promptQuestionRepository;
            _mediaService = mediaService;
            _promptCategoryRepository = promptCategoryRepository;
            _mediator = mediator;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<CreatePromptAnswerResult>> Handle(CreateCustomPromptCommand request, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"Starting custom prompt creation process for User ID: {request.UserId} in Category ID: {request.PromptCategoryId}");

            // Validate that at least one answer format is provided
            if (string.IsNullOrWhiteSpace(request.TextAnswer) && request.VoiceNote == null && request.VideoClip == null)
            {
                _loggerManager.LogWarn("No answer format provided. At least one format (Text, Voice, or Video) must be supplied.");
                return Errors.Prompts.AnswerNotProvided;
            }

            var category = await _promptCategoryRepository.GetCategoryByIdAsync(request.PromptCategoryId);
            if (category == null)
            {
                _loggerManager.LogWarn($"Category not found for Category ID: {request.PromptCategoryId}. Aborting custom prompt creation.");
                return Errors.Prompts.CategoryNotFound;
            }

            var customPromptId = PromptIdHelper.GenerateCustomPromptId(request.UserId, category.CategoryName);
            _loggerManager.LogDebug($"Generated Custom Prompt ID: {customPromptId} for User ID: {request.UserId}.");

            var customPrompt = new PromptQuestion
            {
                QuestionText = request.QuestionText,
                IsSystemGenerated = false, // It's user-created
                PromptCategoryId = request.PromptCategoryId,
                CustomPromptId = customPromptId
            };

             _promptQuestionRepository.AddQuestion(customPrompt);
            _loggerManager.LogInfo($"Successfully added custom prompt question with ID: {customPrompt.Id} for User ID: {request.UserId}");

            // Now handle the answer
            var answerCommand = new PromptAnswerCommand(
                    request.UserId,
                    customPrompt.Id,
                    customPromptId,// Use the custom prompt ID
                    request.TextAnswer,
                    request.VoiceNote,
                    request.VideoClip
                );

            _loggerManager.LogInfo($"Forwarding answer command for custom prompt with ID: {customPrompt.Id} to answer handler.");

            // Handle the answer creation
            return await _mediator.Send(answerCommand, cancellationToken);
        }
    }
}
