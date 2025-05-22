using Fliq.Contracts.Profile;

namespace Fliq.Contracts.Recommendations
{
    public record UserRecommendationDto(
      int Id,
      string DisplayName,
      string? ProfileImageUrl,
      int Age,
      string? ProfileDescription,
      LocationDto? Location,
      double RecommendationScore
     );
}
