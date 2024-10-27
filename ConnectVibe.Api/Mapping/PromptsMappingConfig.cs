using Fliq.Application.Prompts.Commands;
using Fliq.Domain.Entities.Prompts;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class PromptsMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PromptAnswerCommand, PromptAnswer>();
        }
    }
}
