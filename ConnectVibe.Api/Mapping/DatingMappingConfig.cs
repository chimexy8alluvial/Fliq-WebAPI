using Fliq.Application.DatingEnvironment.Commands.BlindDateCategory;
using Fliq.Application.DatingEnvironment.Commands.BlindDates;
using Fliq.Application.DatingEnvironment.Commands.SpeedDating;
using Fliq.Application.DatingEnvironment.Common.BlindDateCategory;
using Fliq.Application.DatingEnvironment.Common.BlindDates;
using Fliq.Application.DatingEnvironment.Common.SpeedDating;
using Fliq.Contracts.Dating;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class DatingMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AddBlindDateCategoryRequest, AddBlindDateCategoryCommand>();
            config.NewConfig<AddBlindDateCategoryResult, AddBlindDateCategoryResponse>();

            config.NewConfig<CreateBlindDateRequest, CreateBlindDateCommand>();
            config.NewConfig<CreateBlindDateResult, CreateBlindDateResponse>();

            config.NewConfig<StartBlindDateRequest, StartBlindDateCommand>();
            config.NewConfig<StartBlindDateResult, StartBlindDateResponse>();

            config.NewConfig<EndBlindDateRequest, EndBlindDateCommand>();
            config.NewConfig<EndBlindDateResult, EndBlindDateResponse>();

            config.NewConfig<JoinBlindDateRequest, JoinBlindDateCommand>();

            //----- Speed-Dating-----//
            config.NewConfig<CreateSpeedDatingEventRequest, CreateSpeedDatingEventCommand>();
            config.NewConfig<CreateSpeedDatingEventResult, CreateSpeedDatingEventResponse>();

            config.NewConfig<StartSpeedDatingEventRequest, StartSpeedDatingEventCommand>();
            config.NewConfig<StartSpeedDatingEventResult, StartSpeedDatingEventResponse>();

            config.NewConfig<JoinSpeedDatingEventRequest, JoinSpeedDatingEventCommand>();

            config.NewConfig<EndSpeedDatingEventRequest, EndSpeedDatingEventCommand>();
            config.NewConfig<EndSpeedDatingEventResult, EndSpeedDatingEventResponse>();
        }
    }
}
