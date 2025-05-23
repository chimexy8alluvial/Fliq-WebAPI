using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;

namespace Fliq.Application.Recommendations.Common
{
    public record ScoredSpeedDateRecommendation(SpeedDatingEvent SpeedDate, double Score);
}
