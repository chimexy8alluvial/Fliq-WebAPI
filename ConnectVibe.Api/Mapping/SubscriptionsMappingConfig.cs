using Fliq.Application.Subscriptions.Commands;
using Fliq.Application.Subscriptions.Queries;
using Fliq.Contracts.Subscriptions;
using Fliq.Domain.Entities.Subscriptions;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class SubscriptionsMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateSubscriptionPlanRequestDto, CreateSubscriptionPlanCommand>();
            config.NewConfig<AddSubscriptionPlanPriceRequestDto, AddSubscriptionPlanPriceCommand>();
            config.NewConfig<SubscriptionPlan, SubscriptionPlanDto>();
        }
    }
}
