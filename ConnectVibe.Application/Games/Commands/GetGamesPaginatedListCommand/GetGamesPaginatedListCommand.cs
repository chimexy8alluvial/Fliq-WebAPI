
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Contracts.Games;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.Games.Commands.GetAllGamesPaginatedListCommand
{
    public record GetGamesPaginatedListCommand(
        int Page,
        int PageSize,
        DateTime? DatePlayedFrom,
        DateTime? DatePlayedTo,
        GameStatus? Status
    ) : IRequest<ErrorOr<GetGamesListResponse>>;

    public class GetGamesPaginatedListCommandHandler : IRequestHandler<GetGamesPaginatedListCommand, ErrorOr<GetGamesListResponse>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILoggerManager _logger;
        public GetGamesPaginatedListCommandHandler(IGamesRepository gamesRepository, ILoggerManager logger)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
        }
        public async Task<ErrorOr<GetGamesListResponse>> Handle(GetGamesPaginatedListCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all games...");
            var (allGames, totalCount) = await _gamesRepository.GetAllGamesListAsync(command.Page, command.PageSize, command.DatePlayedFrom, command.DatePlayedTo, (int?)command.Status);

            if (allGames == null || !allGames.Any())
            {
                _logger.LogError("No games found matching the provided filters");
                return Errors.Games.NoGamesFound;
            }

            var pagedGames = allGames.Select(g => new GamesListItem
            {
                GameTitle = g.GameTitle,
                Players = g.Players,
                Status = g.Status,
                Stake = g.Stake,
                Winner = g.Winner,
                DatePlayed = g.DatePlayed
            }).ToList();

            _logger.LogInfo($"Returning {pagedGames.Count} games out of {totalCount} total matching filters");

            return new GetGamesListResponse(pagedGames, totalCount, command.Page, command.PageSize);
        }
    }
}


