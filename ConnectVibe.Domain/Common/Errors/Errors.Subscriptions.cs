
using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Subscription
        {
            public static Error DuplicateSubscriptionPlan => Error.Conflict(
            code: "SubscriptionPlan.AlreadyExist",
            description: "Subscription Plan already exists");
            public static Error DuplicateSubscriptionPlanPrice => Error.Conflict(
          code: "SubscriptionPlanPrice.AlreadyExist",
          description: "Subscription Plan Price already exists");


        }
    }
}
