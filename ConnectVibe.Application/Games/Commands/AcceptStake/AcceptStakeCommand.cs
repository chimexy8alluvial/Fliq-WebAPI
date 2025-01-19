using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Games;
using MediatR;
using Fliq.Domain.Common.Errors;

namespace Fliq.Application.Games.Commands.AcceptStake
{
    public record AcceptStakeCommand(int StakeId, int UserId) : IRequest<ErrorOr<Stake>>;

    public class AcceptStakeCommandHandler : IRequestHandler<AcceptStakeCommand, ErrorOr<Stake>>
    {
        private readonly IStakeRepository _stakeRepository;
        private readonly IWalletRepository _walletRepository;

        public AcceptStakeCommandHandler(IStakeRepository stakeRepository, IWalletRepository walletRepository)
        {
            _stakeRepository = stakeRepository;
            _walletRepository = walletRepository;
        }

        public async Task<ErrorOr<Stake>> Handle(AcceptStakeCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the stake
            var stake = _stakeRepository.GetStakeById(request.StakeId);
            if (stake == null)
            {
                return Errors.Stake.NotFound;
            }

            // Validate the recipient
            if (stake.RecipientId != request.UserId)
            {
                return Errors.Stake.InvalidRecipient;
            }

            // Check if the stake has already been accepted
            if (stake.IsAccepted)
            {
                return Errors.Stake.AlreadyAccepted;
            }

            // Validate recipient's wallet balance
            var recipientWallet = _walletRepository.GetWalletByUserId(request.UserId);
            if (recipientWallet == null || recipientWallet.Balance < stake.Amount)
            {
                return Errors.Wallet.InsufficientBalance;
            }

            // Deduct amount from recipient's wallet
            recipientWallet.Balance -= stake.Amount;
            _walletRepository.UpdateWallet(recipientWallet);

            // Mark the stake as accepted
            stake.IsAccepted = true;
            _stakeRepository.UpdateStake(stake);

            return stake;
        }
    }
}