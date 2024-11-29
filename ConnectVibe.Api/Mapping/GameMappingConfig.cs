using Fliq.Application.Games.Commands.AcceptGameRequest;
using Fliq.Application.Games.Commands.CreateGame;
using Fliq.Application.Games.Commands.SendGameRequest;
using Fliq.Application.Games.Commands.SubmitAnswer;
using Fliq.Contracts.Games;
using Fliq.Domain.Entities.Games;
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

            config.NewConfig<SendGameRequestDto, SendGameRequestCommand>()
                .IgnoreNullValues(true);

            config.NewConfig<AcceptGameRequestDto, AcceptGameRequestCommand>()
                .IgnoreNullValues(true);

            config.NewConfig<SubmitAnswerDto, SubmitAnswerCommand>()
                .IgnoreNullValues(true);

            // Queries to Responses
            config.NewConfig<Game, GetGameResponse>()
                .IgnoreNullValues(true);

            config.NewConfig<GameSession, GetGameSessionResponse>()
                .IgnoreNullValues(true);

            config.NewConfig<Game, GetGameResponse>()
                .IgnoreNullValues(true); // Mapping from Game entity to GetGameResponse
        }
    }
}