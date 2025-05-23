using Fliq.Domain.Entities.DatingEnvironment.BlindDates;

namespace Fliq.Application.Recommendations.Common
{
    public record ScoredBlindDateRecommendation(BlindDate BlindDate, double Score);
}
