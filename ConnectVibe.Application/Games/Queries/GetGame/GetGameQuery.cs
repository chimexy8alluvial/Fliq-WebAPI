using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Games.Common;
using MediatR;
using Fliq.Domain.Common.Errors;
using Fliq.Application.Common.Interfaces.Services;

namespace Fliq.Application.Games.Queries.GetGame
{
    public record GetGameByIdQuery(int Id) : IRequest<ErrorOr<GetGameResult>>;

    public class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQuery, ErrorOr<GetGameResult>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILoggerManager _logger;

        public GetGameByIdQueryHandler(IGamesRepository gamesRepository, ILoggerManager logger)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<GetGameResult>> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Getting game with id: {request.Id}");
            await Task.CompletedTask;

            var game = _gamesRepository.GetGameById(request.Id);

            if (game is null)
            {
                _logger.LogError($"Game with id: {request.Id} not found");
                return Errors.Games.GameNotFound;
            }

            _logger.LogInfo($"Got game with id: {request.Id}");
            return new GetGameResult(game);
        }
    }
}