using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Recommendations.Common
{
    public record ScoredEventRecommendation(Events Event, double Score);
}
