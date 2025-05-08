using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.Prompts.Commands.AddSystemPrompt
{
    public record ApprovePromptCommand(int PromptId, int AdminUserId) : IRequest<ErrorOr<Unit>>;
    public class ApprovePromptCommandHandler : IRequestHandler<ApprovePromptCommand, ErrorOr<Unit>>
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPromptQuestionRepository _promptQuestionRepository;

        public ApprovePromptCommandHandler(
            ILoggerManager logger,
            IUserRepository userRepository,
            IPromptQuestionRepository promptQuestionRepository)
        {

            _logger = logger;
            _userRepository = userRepository;
            _promptQuestionRepository = promptQuestionRepository;
        }

        public async Task<ErrorOr<Unit>> Handle(ApprovePromptCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Approving prompt with ID: {command.PromptId}");
            var promptQuestion =  _promptQuestionRepository.GetQuestionByIdAsync(command.PromptId);
            if (promptQuestion == null)
            {
                _logger.LogError($"Prompt with ID: {command.PromptId} was not found.");
                return Errors.Prompts.QuestionNotFound;
            }

            if (promptQuestion.ContentCreationStatus == ContentCreationStatus.Approved)
            {
                _logger.LogError($"Prompt with ID: {command.PromptId} has been approved already.");
                return Errors.Prompts.PromptAlreadyApproved;
            }

            var user = _userRepository.GetUserById(command.AdminUserId); //update this to get user by id and role for faster fetch
            if (user == null)
            {
                _logger.LogError($"Admin with Id: {command.AdminUserId} was not found.");
                return Errors.User.UserNotFound;
            }

            promptQuestion.ContentCreationStatus = ContentCreationStatus.Approved;

             _promptQuestionRepository.AddQuestion(promptQuestion); //update

            _logger.LogInfo($"Prompt with ID: {command.PromptId} was approved");

            return Unit.Value;
        }

    }
}
