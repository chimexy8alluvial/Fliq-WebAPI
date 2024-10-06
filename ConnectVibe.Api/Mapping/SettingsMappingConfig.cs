using Fliq.Application.Settings.Commands.Update;
using Fliq.Application.Settings.Common;
using Fliq.Contracts.Settings;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Settings;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class SettingsMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UpdateSettingsRequest, UpdateSettingsCommand>();
            config.NewConfig<GetSettingsResult, GetSettingsResponse>();
            config.NewConfig<NotificationPreferenceDto, NotificationPreference>();
            config.NewConfig<FilterDto, Filter>()
                .Map(dest => dest.LookingFor, src => (LookingFor)src.LookingFor)
                .Map(dest => dest.RacePreferences, src => src.RacePreferences.Select(e => (EthnicityType)e).ToList());
            config.NewConfig<AgeRangeDto, AgeRange>();
            config.NewConfig<ViceDto, Vice>();
        }
    }
}