using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Games;
using MediatR;
using Fliq.Domain.Common.Errors;
using Fliq.Application.Common.Interfaces.Services;

namespace Fliq.Application.Games.Commands.CreateStake
{
    public record CreateStakeCommand(
    int GameSessionId,
    int RequesterId,
    int RecipientId,
    decimal Amount
) : IRequest<ErrorOr<Stake>>;

    public class CreateStakeCommandHandler : IRequestHandler<CreateStakeCommand, ErrorOr<Stake>>
    {
        private readonly IStakeRepository _stakeRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ILoggerManager _logger;

        public CreateStakeCommandHandler(IStakeRepository stakeRepository, IWalletRepository walletRepository, ILoggerManager logger)
        {
            _stakeRepository = stakeRepository;
            _walletRepository = walletRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<Stake>> Handle(CreateStakeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Creating stake");
            // Validate requester wallet balance
            var requesterWallet = _walletRepository.GetWalletByUserId(request.RequesterId);
            if (requesterWallet == null || requesterWallet.Balance < request.Amount)
            {
                _logger.LogError("Insufficient balance");
                return Errors.Wallet.InsufficientBalance;
            }

            // Update requester wallet
            requesterWallet.Balance -= request.Amount;
            _walletRepository.UpdateWallet(requesterWallet);

            var stake = new Stake
            {
                GameSessionId = request.GameSessionId,
                RequesterId = request.RequesterId,
                RecipientId = request.RecipientId,
                Amount = request.Amount
            };

            _stakeRepository.Add(stake);

            _logger.LogInfo($"Stake created: {stake.Id}");
            return stake;
        }
    }
}