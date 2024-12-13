using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Games.Common;
using Fliq.Domain.Entities.Games;
using MediatR;
using Fliq.Domain.Common.Errors;

namespace Fliq.Application.Games.Commands.SendGameRequest
{
    public record SendGameRequestCommand(int GameId, int RequesterId, int RecipientId)
      : IRequest<ErrorOr<GetGameRequestResult>>;

    public class SendGameRequestCommandHandler : IRequestHandler<SendGameRequestCommand, ErrorOr<GetGameRequestResult>>
    {
        private readonly IGamesRepository _gameRequestRepository;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;

        public SendGameRequestCommandHandler(IGamesRepository gameRequestRepository, ILoggerManager logger, IUserRepository userRepository)
        {
            _gameRequestRepository = gameRequestRepository;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<ErrorOr<GetGameRequestResult>> Handle(SendGameRequestCommand request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Sending game request: {request.GameId}");
            var gameRequest = new GameRequest
            {
                GameId = request.GameId,
                RequesterId = request.RequesterId,
                RecipientId = request.RecipientId
            };

            var reciever = _userRepository.GetUserById(request.RecipientId);
            if (reciever is null)
            {
                _logger.LogError("User not found");
                return Errors.User.UserNotFound;
            }
            // Notification

            _gameRequestRepository.AddGameRequest(gameRequest);
            _logger.LogInfo($"Game request sent: {request.GameId}");

            return new GetGameRequestResult(gameRequest);
        }
    }
}