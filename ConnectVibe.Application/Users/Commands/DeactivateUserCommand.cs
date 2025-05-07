
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.Users.Commands
{
    public record DeactivateUserCommand(int UserId, int AdminUserId) : IRequest<ErrorOr<Unit>>;

    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;
        private readonly IAuditTrailService _auditTrailService;

        public DeactivateUserCommandHandler(IUserRepository userRepository, ILoggerManager logger, IAuditTrailService auditTrailService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _auditTrailService = auditTrailService;
        }

        public async Task<ErrorOr<Unit>> Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Starting Deactivation process for user with ID {command.UserId}");
            var AdminUser = _userRepository.GetUserById(command.AdminUserId);

            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                _logger.LogError($"User with ID {command.UserId} not found");
                return Errors.User.UserNotFound;
            }

            user.IsActive = false;

            _userRepository.Update(user);
            _logger.LogInfo($"User with ID {command.UserId} Deactivated successfully");

            var Message = $"Deactivating user with id {user.Id}";
            await _auditTrailService.LogAuditTrail(Message, AdminUser);

            return Unit.Value;
        }
    }
}
