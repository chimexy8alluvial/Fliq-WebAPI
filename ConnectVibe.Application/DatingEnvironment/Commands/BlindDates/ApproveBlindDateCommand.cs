using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.DatingEnvironment.Commands.BlindDates
{
    public record ApproveBlindDateCommand(int BlindDateId, int AdminUserId) : IRequest<ErrorOr<Unit>>;
    public class ApproveBlindDateCommandHandler : IRequestHandler<ApproveBlindDateCommand, ErrorOr<Unit>>
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IBlindDateRepository _blindDateRepository;

        public ApproveBlindDateCommandHandler(
            ILoggerManager logger,
            IUserRepository userRepository,
            IBlindDateRepository blindDateRepository)
        {

            _logger = logger;
            _userRepository = userRepository;
            _blindDateRepository = blindDateRepository;
        }

        public async Task<ErrorOr<Unit>> Handle(ApproveBlindDateCommand command, CancellationToken cancellationToken)
        {

            _logger.LogInfo($"Approving blind date with ID: {command.BlindDateId}");
            var blindDate = await _blindDateRepository.GetByIdAsync(command.BlindDateId);
            if (blindDate == null)
            {
                _logger.LogError($"Blind date with ID: {command.BlindDateId} was not found.");
                return Errors.Dating.BlindDateNotFound;
            }

            if (blindDate.Status != DateStatus.Pending)
            {
                _logger.LogError($"Blind date with ID: {command.BlindDateId} has been approved already.");
                return Errors.Dating.DateAlreadyApproved;
            }

            var user = _userRepository.GetUserById(command.AdminUserId); //update this to get user by id and role for faster fetch
            if (user == null)
            {
                _logger.LogError($"Admin with Id: {command.AdminUserId} was not found.");
                return Errors.User.UserNotFound;
            }

            blindDate.Status = DateStatus.Upcoming;

            await _blindDateRepository.UpdateAsync(blindDate);

            _logger.LogInfo($"Blind Date with ID: {command.BlindDateId} was approved");

            return Unit.Value;
        }

    }
}
