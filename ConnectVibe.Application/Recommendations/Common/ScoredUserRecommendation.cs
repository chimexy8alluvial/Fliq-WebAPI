using Fliq.Domain.Entities;

namespace Fliq.Application.Recommendations.Common
{
    public record ScoredUserRecommendation(User User, double Score);
}
