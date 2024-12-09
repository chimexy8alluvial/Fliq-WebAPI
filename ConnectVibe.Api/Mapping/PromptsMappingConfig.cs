using Fliq.Application.Prompts.Commands;
using Fliq.Application.Prompts.Common;
using Fliq.Contracts.Prompts;
using Fliq.Domain.Entities.Prompts;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class PromptsMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
          //  config.NewConfig<PromptAnswerCommand, PromptAnswer>();
           // config.NewConfig<CreatePromptAnswerRequest, PromptAnswerCommand>();
            config.NewConfig<CreatePromptAnswerResult, CreatePromptAnswerResponse>().Map(dest => dest.Success, src => src.IsAnswered);
         //   config.NewConfig<CreateCustomPromptRequest, CreateCustomPromptCommand>();
            config.NewConfig<CreatePromptAnswerResult, CreateCustomPromptResponse>().Map(dest => dest.Success, src => src.IsAnswered);
        }
    }
}
