using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using Fliq.Infrastructure.Persistence.Repositories;
using MediatR;

namespace Fliq.Application.DatingEnvironment.Commands.SpeedDating
{
    public record RejectSpeedDateCommand(int SpeedDateId, int AdminUserId) : IRequest<ErrorOr<Unit>>;

    public class RejectSpeedDateCommandHandler : IRequestHandler<RejectSpeedDateCommand, ErrorOr<Unit>>
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly ISpeedDatingEventRepository _speedDateRepository;

        public RejectSpeedDateCommandHandler(
            ILoggerManager logger,
            IUserRepository userRepository,
            ISpeedDatingEventRepository speedDateRepository)
        {

            _logger = logger;
            _userRepository = userRepository;
            _speedDateRepository = speedDateRepository;
        }

        public async Task<ErrorOr<Unit>> Handle(RejectSpeedDateCommand command, CancellationToken cancellationToken)
        {

            _logger.LogInfo($"Rejecting speed date with ID: {command.SpeedDateId}");
            var speedDate = await _speedDateRepository.GetByIdAsync(command.SpeedDateId);
            if (speedDate == null)
            {
                _logger.LogError($"Speed date with ID: {command.SpeedDateId} was not found.");
                return Errors.Dating.SpeedDateNotFound;
            }

            if (speedDate.ContentCreationStatus == (int)ContentCreationStatus.Rejected)
            {
                _logger.LogError($"Speed date with ID: {command.SpeedDateId} has been rejected already.");
                return Errors.Dating.DateAlreadyRejected;
            }

            var user = _userRepository.GetUserById(command.AdminUserId); //update this to get user by id and role for faster fetch
            if (user == null)
            {
                _logger.LogError($"Admin with Id: {command.AdminUserId} was not found.");
                return Errors.User.UserNotFound;
            }

            speedDate.ContentCreationStatus = (int)ContentCreationStatus.Rejected;

            await _speedDateRepository.UpdateAsync(speedDate);

            _logger.LogInfo($"Speed Date with ID: {command.SpeedDateId} was rejected");

            return Unit.Value;
        }

    }
}
