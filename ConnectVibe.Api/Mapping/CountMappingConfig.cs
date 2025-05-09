﻿using Fliq.Application.DashBoard.Common;
using Fliq.Contracts.DashBoard;
using Mapster;

namespace Fliq.Api.Mapping
{

   
    public class DashBoardCountMappingConfig : IRegister

    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CountResult, CountResponse>().Map(dest => dest.Count, src => src.Count);

            config.NewConfig<WeeklyCountResult, WeeklyCountResponse>()
                 .Map(dest => dest.DailyCounts,
                      src => src.DailyCounts.ToDictionary(
                          kv => kv.Key.ToString(),
                          kv => kv.Value));
        }
    }
}
