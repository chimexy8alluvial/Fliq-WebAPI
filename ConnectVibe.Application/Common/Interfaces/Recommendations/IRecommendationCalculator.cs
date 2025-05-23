using Fliq.Domain.Entities;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Recommendations;

namespace Fliq.Application.Common.Interfaces.Recommendations
{
    public interface IRecommendationCalculator
    {
        double CalculateEventScore(Events @event, User user, List<UserInteraction> pastInteractions);
        double CalculateBlindDateScore(BlindDate blindDate, User user, List<UserInteraction> pastInteractions);
        double CalculateSpeedDateScore(SpeedDatingEvent speedDate, User user, List<UserInteraction> pastInteractions);
        double CalculateUserMatchScore(User targetUser, User currentUser, List<UserInteraction> pastInteractions);
    }
}
