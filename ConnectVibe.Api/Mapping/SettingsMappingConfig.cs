using Fliq.Application.Settings.Commands.Update;
using Fliq.Application.Settings.Common;
using Fliq.Contracts.Settings;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Settings;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class SettingsMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UpdateSettingsRequest, UpdateSettingsCommand>().IgnoreNullValues(true);
            config.NewConfig<GetSettingsResult, GetSettingsResponse>();
            config.NewConfig<NotificationPreferenceDto, NotificationPreference>().IgnoreNullValues(true);
            config.NewConfig<FilterDto, Filter>().IgnoreNullValues(true)
                .Map(dest => dest.LookingFor, src => (LookingFor)src.LookingFor)
                .Map(dest => dest.RacePreferences, src => src.RacePreferences.Select(e => (EthnicityType)e).ToList());
            config.NewConfig<AgeRangeDto, AgeRange>().IgnoreNullValues(true);
            config.NewConfig<ViceDto, Vice>().IgnoreNullValues(true);
        }
    }
}