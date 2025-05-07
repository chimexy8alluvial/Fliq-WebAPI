using Fliq.Application.PlatformCompliance.Commands;
using Fliq.Application.PlatformCompliance.Commands.CreateCompliance;
using Fliq.Contracts.PlatformCompliance;
using Fliq.Domain.Enums;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class ComplianceMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateComplianceRequest, CreateComplianceCommand>()
                .Map(dest => dest.Language, src => (Language)src.Language);

            config.NewConfig<RecordUserConsentRequest, RecordUserConsentCommand>();
        }
    }
}
