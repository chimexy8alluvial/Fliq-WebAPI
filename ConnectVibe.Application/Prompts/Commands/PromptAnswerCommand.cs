using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.Prompts.Common;
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

        public PromptAnswerCommandHandler(IImageService mediaService, IPromptAnswerRepository promptAnswerRepository, IPromptQuestionRepository promptQuestionRepository, IMapper mapper)
        {
            _mediaService = mediaService;
            _promptAnswerRepository = promptAnswerRepository;
            _promptQuestionRepository = promptQuestionRepository;
            _mapper = mapper;
        }

        public async Task<ErrorOr<CreatePromptAnswerResult>> Handle(PromptAnswerCommand request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(request.TextAnswer) && request.VoiceNote == null && request.VideoClip == null)
            {
                return Errors.Prompts.AnswerNotProvided;
            }

            var promptQuestion = await _promptQuestionRepository.GetQuestionByIdAsync(request.PromptQuestionId);
            if(promptQuestion is null)
            {
                return Errors.Prompts.QuestionNotFound;
            }

            var promptAnswer = _mapper.Map<PromptAnswer>(request);

            if (request.VoiceNote != null)
            {
                promptAnswer.VoiceNoteUrl = await UploadPromptAnswerAsync(request.VoiceNote, PromptAnswerMediaType.VoiceNote);
            }

            if (request.VideoClip != null)
            {
                promptAnswer.VideoClipUrl = await UploadPromptAnswerAsync(request.VideoClip, PromptAnswerMediaType.VideoClip);
            }

             _promptAnswerRepository.Add(promptAnswer);

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

            return await _mediaService.UploadMediaAsync(file, containerName);
        }

    }


}
