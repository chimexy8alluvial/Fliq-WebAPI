using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Games;
using MediatR;

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
        private readonly IGamesRepository _gamesRepository;

        public CreateStakeCommandHandler(IStakeRepository stakeRepository, IWalletRepository walletRepository, ILoggerManager logger, IGamesRepository gamesRepository)
        {
            _stakeRepository = stakeRepository;
            _walletRepository = walletRepository;
            _logger = logger;
            _gamesRepository = gamesRepository;
        }

        public async Task<ErrorOr<Stake>> Handle(CreateStakeCommand request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            _logger.LogInfo("Creating stake");

            //Get game session
            var gameSession = _gamesRepository.GetGameSessionById(request.GameSessionId);
            if (gameSession == null)
            {
                _logger.LogError("Game session not found");
                return Errors.Games.GameSessionNotFound;
            }

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

            //update gamesession
            gameSession.Stake = stake;
            _gamesRepository.UpdateGameSession(gameSession);
            _logger.LogInfo($"Stake created: {stake.Id}");
            return stake;
        }
    }
}