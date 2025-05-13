using Dapper;
using Fliq.Application.Games.Commands.AcceptGameRequest;
using Fliq.Application.Games.Commands.CreateGame;
using Fliq.Application.Games.Commands.CreateStake;
using Fliq.Application.Games.Commands.GetAllGamesPaginatedListCommand;
using Fliq.Application.Games.Commands.SendGameRequest;
using Fliq.Application.Games.Commands.SubmitAnswer;
using Fliq.Application.Games.Common;
using Fliq.Contracts.Games;
using Fliq.Domain.Entities.Games;
using Fliq.Infrastructure.Persistence.Repositories;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class GamesMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Commands
            config.NewConfig<CreateGameDto, CreateGameCommand>()
                .IgnoreNullValues(true);

            config.NewConfig<GetGameResult, GetGameResponse>()
                                .IgnoreNullValues(true);

            config.NewConfig<SendGameRequestDto, SendGameRequestCommand>()
                .IgnoreNullValues(true)
                .Map(dest => dest.ReceiverUserId, src => src.ReceiverUserId)
                .Map(dest => dest.GameDisconnectionResolutionOption, src => (GameDisconnectionResolutionOption)src.GameDisconnectionResolutionOption);

            config.NewConfig<AcceptGameRequestDto, AcceptGameRequestCommand>()
                .IgnoreNullValues(true);

            config.NewConfig<SubmitAnswerDto, SubmitAnswerCommand>()
                .IgnoreNullValues(true);

            config.NewConfig<GetGamesListRequest, GetGamesPaginatedListCommand>()
                .IgnoreNullValues(true);

            // Queries to Responses
            config.NewConfig<Game, GetGameResponse>()
                .IgnoreNullValues(true);

            config.NewConfig<GameSession, GetGameSessionResponse>()
                .IgnoreNullValues(true);
            config.NewConfig<GetGameResult, GetGameResponse>()
                .Map(dest => dest, src => src);

            config.NewConfig<Game, GetGameResponse>()
                .IgnoreNullValues(true); // Mapping from Game entity to GetGameResponse
            config.NewConfig<GetGameHistoryResult, GameSessionResponse>()
               .Map(dest => dest.GameId, src => src.GameId)
               .Map(dest => dest.Player1Id, src => src.Player1Id)
               .Map(dest => dest.Player2Id, src => src.Player2Id)
               .Map(dest => dest.Player1Score, src => src.Player1Score)
               .Map(dest => dest.Player2Score, src => src.Player2Score)
               .Map(dest => dest.Status, src => src.Status)
               .Map(dest => dest.StartTime, src => src.StartTime)
               .Map(dest => dest.EndTime, src => src.EndTime);
            config.NewConfig<GetGameSessionResult, GameSessionResponse>()
                .Map(dest => dest, src => src.GameSession)
                .Map(dest => dest.StakeAmount, src => src.GameSession.Stake != null ? src.GameSession.Stake.Amount : (decimal?)null);
            config.NewConfig<CreateStakeRequestDto, CreateStakeCommand>()
                .Map(dest => dest.ResolutionOption, src => (StakeResolutionOption)src.ResolutionOption)
                .IgnoreNullValues(true);
            config.NewConfig<Stake, StakeResponseDto>()
                .IgnoreNullValues(true);

            SqlMapper.AddTypeHandler(typeof(List<string>), new JsonListTypeHandler());
        }
    }
}