using ErrorOr;
using Fliq.Application.Prompts.Common;
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
        public Task<ErrorOr<CreatePromptAnswerResult>> Handle(PromptAnswerCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
