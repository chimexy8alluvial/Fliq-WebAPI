﻿using ErrorOr;
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
            var blindDate = await _speedDateRepository.GetByIdAsync(command.SpeedDateId);
            if (blindDate == null)
            {
                _logger.LogError($"Speed date with ID: {command.SpeedDateId} was not found.");
                return Errors.Dating.SpeedDateNotFound;
            }

            if (blindDate.Status != DateStatus.Pending)
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

            blindDate.Status = DateStatus.Upcoming;

            await _speedDateRepository.UpdateAsync(blindDate);

            _logger.LogInfo($"Speed Date with ID: {command.SpeedDateId} was rejected");

            return Unit.Value;
        }

    }
}
