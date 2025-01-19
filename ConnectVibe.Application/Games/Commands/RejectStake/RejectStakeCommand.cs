using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Games;
using MediatR;
using Fliq.Domain.Common.Errors;
using Fliq.Application.Common.Interfaces.Services;

namespace Fliq.Application.Games.Commands.RejectStake
{
    public record RejectStakeCommand(int StakeId, int UserId) : IRequest<ErrorOr<Stake>>;

    public class RejectStakeCommandHandler : IRequestHandler<RejectStakeCommand, ErrorOr<Stake>>
    {
        private readonly IStakeRepository _stakeRepository;
        private readonly ILoggerManager _logger;

        public RejectStakeCommandHandler(IStakeRepository stakeRepository, ILoggerManager logger)
        {
            _stakeRepository = stakeRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<Stake>> Handle(RejectStakeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Rejecting stake {request.StakeId}");
            // Fetch the stake
            var stake = _stakeRepository.GetStakeById(request.StakeId);
            if (stake == null)
            {
                _logger.LogError($"Stake {request.StakeId} not found");
                return Errors.Stake.NotFound;
            }
            if (stake.RecipientId != request.UserId)
            {
                _logger.LogError($"Stake {request.StakeId} does not belong to user {request.UserId}");
                return Errors.Stake.InvalidRecipient;
            }

            if (stake.IsAccepted)
            {
                _logger.LogError($"Stake {request.StakeId} has already been accepted");
                return Errors.Stake.AlreadyAccepted;
            }
            // Reject the stake
            stake.IsAccepted = false;
            _stakeRepository.UpdateStake(stake);

            return stake;
        }
    }
}