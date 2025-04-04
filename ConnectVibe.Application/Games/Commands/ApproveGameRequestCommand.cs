

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.Games.Commands
{
    public record ApproveGameRequestCommand(int GameRequestId, int AdminUserId) : IRequest<ErrorOr<Unit>>;
    public class ApproveGameRequestCommandHandler : IRequestHandler<ApproveGameRequestCommand, ErrorOr<Unit>>
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IGamesRepository _gameRepository;

        public ApproveGameRequestCommandHandler(
            ILoggerManager logger,
            IUserRepository userRepository,
            IGamesRepository gameRepository)
        {

            _logger = logger;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
        }

        public async Task<ErrorOr<Unit>> Handle(ApproveGameRequestCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Approving Game with ID: {command.GameRequestId}");
            var game =  _gameRepository.GetGameById(command.GameRequestId);
            if (game == null)
            {
                _logger.LogError($"Game with ID: {command.GameRequestId} was not found.");
                return Errors.Games.GameNotFound;
            }

            if (game.CreationStatus == GameCreationStatus.Approved)
            {
                _logger.LogError($"Game with ID: {command.GameRequestId} has been approved already.");
                return Errors.Games.GameApproved;
            }

            var user = _userRepository.GetUserById(command.AdminUserId); //update this to get user by id and role for faster fetch
            if (user == null)
            {
                _logger.LogError($"Admin with Id: {command.AdminUserId} was not found.");
                return Errors.User.UserNotFound;
            }

            game.CreationStatus = GameCreationStatus.Approved;

             _gameRepository.UpdateGame(game);

            _logger.LogInfo($"Game with ID: {command.GameRequestId} was approved");

            return Unit.Value;
        }

    }
}
