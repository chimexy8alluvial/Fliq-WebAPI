
//using ErrorOr;
//using Fliq.Application.Common.Interfaces.Persistence;
//using Fliq.Application.Common.Interfaces.Services;
//using Fliq.Contracts.Games;
//using Fliq.Domain.Enums;
//using MediatR;

//namespace Fliq.Application.Games.Commands.GetAllGamesPaginatedListCommand
//{
//    public record GetGamesPaginatedListCommand(
//        int Page,
//        int PageSize,
//        DateTime? DatePlayedFrom,
//        DateTime? DatePlayedTo,
//        GameStatus? Status
//    ) : IRequest<ErrorOr<GetGamesListResponse>>;

//    public class GetGamesPaginatedListCommandHandler : IRequestHandler<GetGamesPaginatedListCommand, ErrorOr<GetGamesListResponse>>
//    {
//        private readonly IGamesRepository _gamesRepository;
//        private readonly ILoggerManager _logger;
//        public GetGamesPaginatedListCommandHandler(IGamesRepository gamesRepository, ILoggerManager logger)
//        {
//            _gamesRepository = gamesRepository;
//            _logger = logger;
//        }
//        public async Task<ErrorOr<GetGamesListResponse>> Handle(GetGamesPaginatedListCommand command, CancellationToken cancellationToken)
//        {
//            _logger.LogInfo($"Get all games list command received. Page: {command.Page}, PageSize: {command.PageSize}, DatePlayedFrom: {command.DatePlayedFrom}, DatePlayedTo: {command.DatePlayedTo}, Status: {command.Status}");

//            _logger.LogInfo("Fetching all games...");
//            var (allGames, totalCount) = await _gamesRepository.GetAllGamesListAsync(command.Page, command.PageSize, command.DatePlayedFrom, command.DatePlayedTo, (int?)command.Status);

//            if (allGames == null || !allGames.Any())
//            {
//                _logger.LogError("No games found matching the provided filters");
//                return Error.Games.NoGamesFound;
//            }

//            var pagedGames = allGames.Select(g => new GamesListItem
//            {
//                GameTitle = g.GameTitle,
//                Players = g.Players,
//                Status = g.Status,
//                Stake = g.Stake,
//                Winner = g.Winner,
//                DatePlayed = g.DatePlayed
//            }).ToList();

//            _logger.LogInfo($"Returning {pagedGames.Count} games out of {totalCount} total matching filters");

//            return new GetGamesListResponse(pagedGames, totalCount, command.Page, command.PageSize);
//        }
//    }
//}


//using ErrorOr;
//using Fliq.Application.Common.Interfaces.Persistence;
//using Fliq.Application.Common.Interfaces.Services;
//using Fliq.Contracts.Games;
//using Fliq.Domain.Common.Errors;
//using Fliq.Domain.Enums;
//using MediatR;
//using System.Data.SqlTypes;

//namespace Fliq.Application.Games.Commands.GetGamesList
//{
//    public record GetGamesListCommand(
//        int Page,
//        int PageSize,
//        DateTime? DatePlayedFrom,
//        DateTime? DatePlayedTo,
//        GameStatus? Status
//    ) : IRequest<ErrorOr<GetGamesListResponse>>;
//    public class GetGamesListCommandHandler : IRequestHandler<GetGamesListCommand, ErrorOr<GetGamesListResponse>>
//    {
//        private readonly IGamesRepository _gamesRepository;
//        private readonly ILoggerManager _logger;
//        public GetGamesListCommandHandler(IGamesRepository gamesRepository, ILoggerManager logger)
//        {
//            _gamesRepository = gamesRepository;
//            _logger = logger;
//        }

//        public async Task<ErrorOr<GetGamesListResponse>> Handle(GetGamesListCommand command, CancellationToken cancellationToken)
//        {
//            _logger.LogInfo($"Get all games list command received. Page: {command.Page}, PageSize: {command.PageSize}, DatePlayedFrom: {command.DatePlayedFrom}, DatePlayedTo: {command.DatePlayedTo}, Category: {command.Category}, Location: {command.Location}, Status: {command.Status}");

//            if (command.Page <= 0)
//            {
//                _logger.LogError("Invalid page number provided: Page must be greater than 0");
//                return Errors.Games.InvalidPaginationPage;
//            }
//            if (command.PageSize <= 0)
//            {
//                _logger.LogError("Invalid page size provided: PageSize must be greater than 0");
//                return Errors.Games.InvalidPaginationPageSize;
//            }

//            if (command.DatePlayedFrom.HasValue &&
//                (command.DatePlayedFrom.Value < SqlDateTime.MinValue.Value || command.DatePlayedFrom.Value > SqlDateTime.MaxValue.Value))
//            {
//                _logger.LogError($"Invalid DatePlayedFrom provided: {command.DatePlayedFrom.Value} is outside SQL Server DATETIME range (1753-9999)");
//                return Errors.Games.InvalidDatePlayedFromRange;
//            }
//            if (command.DatePlayedTo.HasValue &&
//                (command.DatePlayedTo.Value < SqlDateTime.MinValue.Value || command.DatePlayedTo.Value > SqlDateTime.MaxValue.Value))
//            {
//                _logger.LogError($"Invalid DatePlayedTo provided: {command.DatePlayedTo.Value} is outside SQL Server DATETIME range (1753-9999)");
//                return Errors.Games.InvalidDatePlayedToRange;
//            }
//            if (command.DatePlayedFrom.HasValue && command.DatePlayedTo.HasValue &&
//                command.DatePlayedFrom.Value > command.DatePlayedTo.Value)
//            {
//                _logger.LogError($"Invalid date range: DatePlayedFrom ({command.DatePlayedFrom.Value}) is after DatePlayedTo ({command.DatePlayedTo.Value})");
//                return Errors.Games.InvalidDateRangeOrder;
//            }

//            _logger.LogInfo("Fetching all games...");
//            var (allGames, totalCount) = await _gamesRepository.GetAllGamesListAsync(command.Page, command.PageSize, command.DatePlayedFrom, command.DatePlayedTo, (int?)command.Status);

//            if (allGames == null || !allGames.Any())
//            {
//                _logger.LogError("No games found matching the provided filters");
//                return Errors.Games.NoGamesFound;
//            }

//            var pagedGames = allGames.Select(g => new GamesListItem
//            {
//                GameTitle = g.GameTitle,
//                Players = g.Players,
//                Status = g.Status,
//                Stake = g.Stake,
//                Winner = g.Winner,
//                DatePlayed = g.DatePlayed
//            }).ToList();

//            _logger.LogInfo($"Returning {pagedGames.Count} games out of {totalCount} total matching filters");

//            return new GetGamesListResponse(pagedGames, totalCount, command.Page, command.PageSize);
//        }
//    }
//}


