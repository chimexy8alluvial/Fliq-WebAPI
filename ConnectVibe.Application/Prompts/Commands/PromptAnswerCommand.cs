using ErrorOr;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.Prompts.Common;
using Fliq.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Prompts.Commands
{
    public record PromptAnswerCommand(
        int UserId,
        int PromptQuestionId,
        string? TextAnswer = null,
        IFormFile? VoiceNote = null,
        IFormFile? VideoClip = null
        ) : IRequest<ErrorOr<CreatePromptAnswerResult>>;


    public class PromptAnswerCommandHandler : IRequestHandler<PromptAnswerCommand, ErrorOr<CreatePromptAnswerResult>>
    {
        private readonly IImageService _mediaService;

        public PromptAnswerCommandHandler(IImageService mediaService)
        {
            _mediaService = mediaService;
        }

        public Task<ErrorOr<CreatePromptAnswerResult>> Handle(PromptAnswerCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
