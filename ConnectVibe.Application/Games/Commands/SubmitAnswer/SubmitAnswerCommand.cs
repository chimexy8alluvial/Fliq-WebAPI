using ErrorOr;
using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Fliq.Application.Games.Commands.SubmitAnswer
{
    public record SubmitAnswerCommand(int SessionId, int Player1Score, int Player2Score, bool Completed, int? LastManId)
       : IRequest<ErrorOr<SubmitAnswerResult>>;

    public class SubmitAnswerCommandHandler : IRequestHandler<SubmitAnswerCommand, ErrorOr<SubmitAnswerResult>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILoggerManager _loggerManager;
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IWalletRepository _walletRepository;
        private readonly IStakeRepository _stakeRepository;

        public SubmitAnswerCommandHandler(IGamesRepository gamesRepository, ILoggerManager loggerManager, IHubContext<GameHub> hubContext, IWalletRepository walletRepository, IStakeRepository stakeRepository)
        {
            _gamesRepository = gamesRepository;
            _loggerManager = loggerManager;
            _hubContext = hubContext;
            _walletRepository = walletRepository;
            _stakeRepository = stakeRepository;
        }

        public async Task<ErrorOr<SubmitAnswerResult>> Handle(SubmitAnswerCommand request, CancellationToken cancellationToken)
        {
            var session = _gamesRepository.GetGameSessionById(request.SessionId);

            if (session == null || session.Status != GameStatus.InProgress)
            {
                _loggerManager.LogError($"Session {request.SessionId} not found");
                return Errors.Games.GameNotFound;
            }

            int? winnerId = null;
            if (session.DisconnectionResolutionOption == GameDisconnectionResolutionOption.LastManWins)
            {
                winnerId = request.LastManId;
                session.WinnerId = request.LastManId;
            }

            // Update game session status and scores
            session.Player1Score = request.Player1Score;
            session.Player2Score = request.Player2Score;
            session.Status = GameStatus.Done;

            _gamesRepository.UpdateGameSession(session);

            // Handle stake resolution if there is a stake
            if (session.Stake != null && session.Stake.StakeStatus == StakeStatus.Pending)
            {
                var stake = session.Stake;
                if (request.Completed) // Game finished normally
                {
                    if (winnerId != null)
                    {
                        await CreditUser(winnerId.Value, stake.Amount);
                        stake.StakeStatus = StakeStatus.Paid;
                    }
                }
                else // Handle disconnection case
                {
                    switch (stake.ResolutionOption)
                    {
                        case StakeResolutionOption.WinnerTakesAll:
                            if (winnerId != null)
                            {
                                await CreditUser(winnerId.Value, stake.Amount);
                                stake.StakeStatus = StakeStatus.Paid;
                            }
                            break;

                        case StakeResolutionOption.ReturnToPlayers:
                            await RefundStake(stake);
                            break;

                        default:
                            break;
                    }
                }

                _stakeRepository.UpdateStake(stake);
            }

            // Notify players via SignalR
            await _hubContext.Clients.Group(session.Id.ToString()).SendAsync("GameEnded", session);

            return new SubmitAnswerResult(session);
        }

        private async Task CreditUser(int userId, decimal amount)
        {
            await Task.CompletedTask;
            var wallet = _walletRepository.GetWalletByUserId(userId);
            if (wallet == null) return;

            wallet.Balance += amount;
            _walletRepository.UpdateWallet(wallet);

            _loggerManager.LogInfo($"Credited {amount} to User {userId}");
        }

        private async Task RefundStake(Stake stake)
        {
            await Task.CompletedTask;
            var requesterWallet = _walletRepository.GetWalletByUserId(stake.RequesterId);
            var recipientWallet = _walletRepository.GetWalletByUserId(stake.RecipientId);

            if (requesterWallet != null)
            {
                requesterWallet.Balance += (stake.Amount / 2);
                _walletRepository.UpdateWallet(requesterWallet);
            }

            if (recipientWallet != null)
            {
                recipientWallet.Balance += (stake.Amount / 2);
                _walletRepository.UpdateWallet(recipientWallet);
            }

            stake.StakeStatus = StakeStatus.Cancelled;
            _loggerManager.LogInfo($"Refunded {stake.Amount} equally to Users {stake.RequesterId} and {stake.RecipientId}");
        }
    }
}