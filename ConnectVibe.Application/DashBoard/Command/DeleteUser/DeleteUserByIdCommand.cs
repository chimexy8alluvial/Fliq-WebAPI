using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.DashBoard.Command.DeleteUser
{
    public record DeleteUserByIdCommand(int UserId, int AdminUserId) : IRequest<ErrorOr<Unit>>;

    public class DeleteUserByIdCommandHandler : IRequestHandler<DeleteUserByIdCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;
        private readonly IAuditTrailService _auditTrailService;

        public DeleteUserByIdCommandHandler(IUserRepository userRepository, ILoggerManager logger, IAuditTrailService auditTrailService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _auditTrailService = auditTrailService;
        }

        public async Task<ErrorOr<Unit>> Handle(DeleteUserByIdCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Deleting user with ID: {command.UserId} ");

            var AdminUser = _userRepository.GetUserById(command.AdminUserId);

            var user = _userRepository.GetUserById(command.UserId);

            if (user == null)
            {
                _logger.LogError("User not found");
                return Errors.User.UserNotFound;
            }

            if (user.IsDeleted)
            {
                _logger.LogInfo($"This user with ID: {user.Id} has been deleted before");
                 return Errors.User.UserAlreadyDeleted;
            }

                user.IsDeleted = true;

            _userRepository.Update(user);

            _logger.LogInfo($"User with ID: {command.UserId} was deleted");

            var Message = $"Deleting user with id {user.Id}";
            await _auditTrailService.LogAuditTrail(Message, AdminUser);

            return  Unit.Value;
            
        }
    }
}
