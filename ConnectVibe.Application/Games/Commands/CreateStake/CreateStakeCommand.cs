using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Games;
using MediatR;

namespace Fliq.Application.Games.Commands.CreateStake
{
    public record CreateStakeCommand(
    int GameSessionId,
    int RequesterId,
    int RecipientId,
    decimal Amount,
    StakeResolutionOption ResolutionOption
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
                _logger.LogError($"Game session with Id {request.GameSessionId} not found");
                return Errors.Games.GameSessionNotFound;
            }

            // Validate requester wallet balance
            var requesterWallet = _walletRepository.GetWalletByUserId(request.RequesterId);
            try
            {
                // Validate requester's wallet balance
                if (requesterWallet == null || requesterWallet.Balance < request.Amount)
                {
                    _logger.LogError($"Insufficient balance for UserId: {request.RequesterId}.");

                    // Log failed wallet history
                    var walletHistory = new WalletHistory
                    {
                        ActivityType = WalletActivityType.Withdrawal,
                        Amount = request.Amount,
                        Description = "Failed withdrawal for stake due to insufficient balance.",
                        WalletId = requesterWallet?.Id ?? 0, // Use 0 or a default value if wallet is null
                        TransactionStatus = WalletTransactionStatus.Failed,
                        FailureReason = "Insufficient balance",
                    };
                    _walletRepository.AddWalletHistory(walletHistory);

                    return Errors.Wallet.InsufficientBalance;
                }

                // Deduct amount from recipient's wallet
                requesterWallet.Balance -= request.Amount;
                _walletRepository.UpdateWallet(requesterWallet);

                // Log successful wallet history
                var successWalletHistory = new WalletHistory
                {
                    ActivityType = WalletActivityType.Withdrawal,
                    Amount = request.Amount,
                    Description = "Successful withdrawal for stake.",
                    WalletId = requesterWallet.Id,
                    TransactionStatus = WalletTransactionStatus.Success,
                };
                _walletRepository.AddWalletHistory(successWalletHistory);

                _logger.LogInfo($"Withdrawal of {request.Amount} for stake successful for UserId: {request.RequesterId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while processing the withdrawal for UserId: {request.RequesterId}. Exception: {ex.Message}");

                // Log the failure in wallet history
                var failureWalletHistory = new WalletHistory
                {
                    ActivityType = WalletActivityType.Withdrawal,
                    Amount = request.Amount,
                    Description = "Failed withdrawal for stake due to an unexpected error.",
                    WalletId = requesterWallet?.Id ?? 0,
                    TransactionStatus = WalletTransactionStatus.Failed,
                    FailureReason = ex.Message
                };
                _walletRepository.AddWalletHistory(failureWalletHistory);

                return Errors.Wallet.UnableTodeduct;
            }

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
            _logger.LogInfo($"Stake created for gamesession with Id: {gameSession.Id}");
            return stake;
        }
    }
}