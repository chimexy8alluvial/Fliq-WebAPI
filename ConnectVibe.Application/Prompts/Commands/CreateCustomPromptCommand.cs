using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
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

        public CreateCustomPromptCommandHandler(IPromptQuestionRepository promptQuestionRepository, IImageService mediaService, IPromptCategoryRepository promptCategoryRepository, ISender mediator)
        {
            _promptQuestionRepository = promptQuestionRepository;
            _mediaService = mediaService;
            _promptCategoryRepository = promptCategoryRepository;
            _mediator = mediator;
        }

        public async Task<ErrorOr<CreatePromptAnswerResult>> Handle(CreateCustomPromptCommand request, CancellationToken cancellationToken)
        {
            // Validate that at least one answer format is provided
            if (string.IsNullOrWhiteSpace(request.TextAnswer) && request.VoiceNote == null && request.VideoClip == null)
            {
                return Errors.Prompts.AnswerNotProvided;
            }

            var category = await _promptCategoryRepository.GetCategoryByIdAsync(request.PromptCategoryId);
            if (category == null)
            {
               return Errors.Prompts.CategoryNotFound;
            }

            var customPromptId = PromptIdHelper.GenerateCustomPromptId(request.UserId, category.CategoryName);

            var customPrompt = new PromptQuestion
            {
                QuestionText = request.QuestionText,
                IsSystemGenerated = false, // It's user-created
                PromptCategoryId = request.PromptCategoryId,
                CustomPromptId = customPromptId
            };

             _promptQuestionRepository.AddQuestion(customPrompt);

            // Now handle the answer
                var answerCommand = new PromptAnswerCommand(
                    request.UserId,
                    customPrompt.Id,
                    customPromptId,// Use the custom prompt ID
                    request.TextAnswer,
                    request.VoiceNote,
                    request.VideoClip
                );

                // Handle the answer creation
               return await _mediator.Send(answerCommand, cancellationToken);
        }
    }
}
