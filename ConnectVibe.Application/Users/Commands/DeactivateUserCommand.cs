
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace Fliq.Application.Users.Commands
{
    public record DeactivateUserCommand(int UserId) : IRequest<ErrorOr<Unit>>;

    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;
        //private readonly HttpContext _httpContext;
        private readonly IAuditTrailRepository _auditTrailRepository;

        public DeactivateUserCommandHandler(IUserRepository userRepository, ILoggerManager logger, IAuditTrailRepository auditTrailRepository)
        {
            _userRepository = userRepository;
            _logger = logger;
            //_httpContext = httpContext;
            _auditTrailRepository = auditTrailRepository;
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

            var auditTrail = new AuditTrail
            {
                UserId = user.Id,
                UserFirstName = user.FirstName,
                UserLastName = user.LastName,
                UserEmail = user.Email,
                UserRole = user.Role.Name,
                AuditAction = $"Deactivating user with id {user.Id}",
                //IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            };

            await _auditTrailRepository.AddAuditTrailAsync(auditTrail);

            return Unit.Value;
        }
    }
}
