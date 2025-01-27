using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Games;
using MediatR;

namespace Fliq.Application.Games.Commands.AcceptStake
{
    public record AcceptStakeCommand(int StakeId, int UserId) : IRequest<ErrorOr<Stake>>;

    public class AcceptStakeCommandHandler : IRequestHandler<AcceptStakeCommand, ErrorOr<Stake>>
    {
        private readonly IStakeRepository _stakeRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ILoggerManager _logger;

        public AcceptStakeCommandHandler(IStakeRepository stakeRepository, IWalletRepository walletRepository, ILoggerManager logger)
        {
            _stakeRepository = stakeRepository;
            _walletRepository = walletRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<Stake>> Handle(AcceptStakeCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the stake
            var stake = _stakeRepository.GetStakeById(request.StakeId);
            if (stake == null)
            {
                _logger.LogError($"Stake with Id: {request.StakeId} not found.");
                return Errors.Stake.NotFound;
            }

            // Validate the recipient
            if (stake.RecipientId != request.UserId)
            {
                _logger.LogError($"Invalid recipient for stake with Id: {request.StakeId}.");
                return Errors.Stake.InvalidRecipient;
            }

            // Check if the stake has already been accepted
            if (stake.IsAccepted)
            {
                _logger.LogError($"Stake with Id: {request.StakeId} has already been accepted.");
                return Errors.Stake.AlreadyAccepted;
            }

            var recipientWallet = _walletRepository.GetWalletByUserId(request.UserId);

            try
            {
                // Validate recipient's wallet balance
                if (recipientWallet == null || recipientWallet.Balance < stake.Amount)
                {
                    _logger.LogError($"Insufficient balance for UserId: {request.UserId}.");

                    // Log failed wallet history
                    var walletHistory = new WalletHistory
                    {
                        ActivityType = WalletActivityType.Withdrawal,
                        Amount = stake.Amount,
                        Description = "Failed withdrawal for stake due to insufficient balance.",
                        WalletId = recipientWallet?.Id ?? 0, // Use 0 or a default value if wallet is null
                        TransactionStatus = WalletTransactionStatus.Failed,
                        FailureReason = "Insufficient balance",
                    };
                    _walletRepository.AddWalletHistory(walletHistory);

                    return Errors.Wallet.InsufficientBalance;
                }

                // Deduct amount from recipient's wallet
                recipientWallet.Balance -= stake.Amount;
                _walletRepository.UpdateWallet(recipientWallet);

                // Log successful wallet history
                var successWalletHistory = new WalletHistory
                {
                    ActivityType = WalletActivityType.Withdrawal,
                    Amount = stake.Amount,
                    Description = "Successful withdrawal for stake.",
                    WalletId = recipientWallet.Id,
                    TransactionStatus = WalletTransactionStatus.Success,
                };
                _walletRepository.AddWalletHistory(successWalletHistory);

                _logger.LogInfo($"Withdrawal of {stake.Amount} for stake successful for UserId: {request.UserId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while processing the withdrawal for UserId: {request.UserId}. Exception: {ex.Message}");

                // Log the failure in wallet history
                var failureWalletHistory = new WalletHistory
                {
                    ActivityType = WalletActivityType.Withdrawal,
                    Amount = stake.Amount,
                    Description = "Failed withdrawal for stake due to an unexpected error.",
                    WalletId = recipientWallet?.Id ?? 0,
                    TransactionStatus = WalletTransactionStatus.Failed,
                    FailureReason = ex.Message
                };
                _walletRepository.AddWalletHistory(failureWalletHistory);

                return Errors.Wallet.UnableTodeduct;
            }

            // Mark the stake as accepted
            stake.IsAccepted = true;
            stake.Amount += stake.Amount;
            _stakeRepository.UpdateStake(stake);

            return stake;
        }
    }
}