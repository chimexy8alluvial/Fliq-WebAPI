
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.Users.Commands
{
    public record DeactivateUserCommand(int UserId) : IRequest<ErrorOr<Unit>>;

    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public DeactivateUserCommandHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<Unit>> Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Starting Deactivation process for user with ID {command.UserId}");
            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                _logger.LogError($"User with ID {command.UserId} not found");
                return Errors.User.UserNotFound;
            }

            user.IsActive = false;

            _userRepository.Update(user);
            _logger.LogInfo($"User with ID {command.UserId} Deactivated successfully");

            return Unit.Value;
        }
    }
}
