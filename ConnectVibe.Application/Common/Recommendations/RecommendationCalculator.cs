using Fliq.Application.Common.Interfaces.Recommendations;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Recommendations;
using Fliq.Domain.Enums;

namespace Fliq.Application.Common.Recommendations
{
    public class RecommendationCalculator : IRecommendationCalculator
    {
        private readonly ILoggerManager _logger;

        public RecommendationCalculator(ILoggerManager logger)
        {
            _logger = logger;
        }

        public double CalculateEventScore(Events @event, User user, List<UserInteraction> pastInteractions)
        {
            _logger.LogInfo($"Calculating score for event: {@event.Id} for user: {user.Id}");
            double score = 0;
            var userProfile = user.UserProfile;

            // 1. Content-based scoring
            // Location proximity
            if (userProfile?.Location != null && @event.Location != null)
            {
                double distance = CalculateDistance(
                    userProfile.Location.Lat, userProfile.Location.Lng,
                    @event.Location.Lat, @event.Location.Lng);

                // Higher score for closer events
                score += Math.Max(0, 10 - distance) / 10;
                _logger.LogInfo($"Location proximity score: {Math.Max(0, 10 - distance) / 10}");
            }

            // Age match
            int userAge = CalculateAge(userProfile.DOB);
            if (userAge >= @event.MinAge && userAge <= @event.MaxAge)
            {
                score += 0.5;
                _logger.LogInfo("Age match: +0.5");
            }

            // 2. Collaborative filtering component
            foreach (var interaction in pastInteractions.Where(i => i.EventId.HasValue))
            {
                if (interaction.Event == null) continue;

                // Higher weight for stronger interactions
                double interactionWeight = interaction.InteractionStrength;

                // Similarity between current event and past interacted event
                double similarity = CalculateEventSimilarity(@event, interaction.Event);

                var collaborativeScore = similarity * interactionWeight;
                score += collaborativeScore;
                _logger.LogInfo($"Collaborative score from event {interaction.EventId}: +{collaborativeScore}");
            }

            // 3. Freshness boost
            if (@event.StartDate < DateTime.Now.AddDays(3))
            {
                score += 0.5; // Boost upcoming events
                _logger.LogInfo("Upcoming event (3 days): +0.5");
            }
            else if (@event.StartDate < DateTime.Now.AddDays(7))
            {
                score += 0.3; // Smaller boost for events within a week
                _logger.LogInfo("Upcoming event (7 days): +0.3");
            }

            // 4. Rating boost
            if (@event.Reviews.Count > 0)
            {
                double avgRating = @event.Reviews.Average(r => r.Rating);
                double ratingBoost = avgRating / 10; // 0-0.5 boost based on ratings
                score += ratingBoost;
                _logger.LogInfo($"Rating boost (avg {avgRating}): +{ratingBoost}");
            }

            // 5. Sponsored boost
            if (@event.SponsoredEvent)
            {
                score += 0.2;
                _logger.LogInfo("Sponsored event: +0.2");
            }

            _logger.LogInfo($"Final score for event {@event.Id}: {score}");
            return score;
        }

        public double CalculateBlindDateScore(BlindDate blindDate, User user, List<UserInteraction> pastInteractions)
        {
            double score = 0;
            var userProfile = user.UserProfile;

            // 1. Location proximity
            if (userProfile?.Location != null && blindDate.Location != null)
            {
                double distance = CalculateDistance(
                    userProfile.Location.Lat, userProfile.Location.Lng,
                    blindDate.Location.Lat, blindDate.Location.Lng);

                // Higher score for closer events
                score += Math.Max(0, 10 - distance) / 10;
            }

            // 2. Creator popularity
            var creatorInteractions = pastInteractions
                .Where(i => i.InteractedWithUserId == blindDate.CreatedByUserId)
                .ToList();

            if (creatorInteractions.Any())
            {
                score += 0.3; // Boost if user has interacted with creator before
            }

            // 3. Format preference (one-on-one vs group)
            var oneOnOnePreference = pastInteractions
                .Where(i => i.BlindDateId != null)
                .Where(i => i.BlindDate?.IsOneOnOne == true)
                .Count();

            var groupPreference = pastInteractions
                .Where(i => i.BlindDateId != null)
                .Where(i => i.BlindDate?.IsOneOnOne == false)
                .Count();

            // Boost based on format preference
            if (blindDate.IsOneOnOne && oneOnOnePreference > groupPreference)
            {
                score += 0.3;
            }
            else if (!blindDate.IsOneOnOne && groupPreference > oneOnOnePreference)
            {
                score += 0.3;
            }

            // 4. Recency boost
            if (blindDate.StartDateTime < DateTime.Now.AddDays(3))
            {
                score += 0.4; // Boost upcoming blind dates
            }
            else if (blindDate.StartDateTime < DateTime.Now.AddDays(7))
            {
                score += 0.2; // Smaller boost for blind dates within a week
            }

            return score;
        }

        public double CalculateSpeedDateScore(SpeedDatingEvent speedDate, User user, List<UserInteraction> pastInteractions)
        {
            double score = 0;
            var userProfile = user.UserProfile;

            // 1. Age match
            int userAge = CalculateAge(userProfile.DOB);
            if (userAge >= speedDate.MinAge && userAge <= speedDate.MaxAge)
            {
                score += 0.5;
            }

            // 2. Location proximity
            if (userProfile?.Location != null && speedDate.Location != null)
            {
                double distance = CalculateDistance(
                    userProfile.Location.Lat, userProfile.Location.Lng,
                    speedDate.Location.Lat, speedDate.Location.Lng);

                // Higher score for closer events
                score += Math.Max(0, 10 - distance) / 10;
            }

            // 3. Category match (sexual orientation)
            // Check if the speed dating category aligns with user's orientation
            if (userProfile.SexualOrientation != null)
            {
                if((userProfile.Gender.GenderType.ToLower() == "male" && userProfile.SexualOrientation.SexualOrientationType.ToLower() == "women" && speedDate.Category == SpeedDatingCategory.Heterosexual) ||
                    (userProfile.Gender.GenderType.ToLower() == "male" && userProfile.SexualOrientation.SexualOrientationType.ToLower() == "men" && speedDate.Category == SpeedDatingCategory.Gay) ||
                    (userProfile.Gender.GenderType.ToLower() == "female" && userProfile.SexualOrientation.SexualOrientationType.ToLower() == "women" && speedDate.Category == SpeedDatingCategory.Lesbian)||
                    (userProfile.Gender.GenderType.ToLower() == "female" && userProfile.SexualOrientation.SexualOrientationType.ToLower() == "men" && speedDate.Category == SpeedDatingCategory.Heterosexual))
                {
                    score += 0.4;
                }
            }

            // 4. Past participation
            var pastSpeedDatingParticipation = pastInteractions
                .Count(i => i.SpeedDatingEventId != null);

            if (pastSpeedDatingParticipation > 0)
            {
                // User has participated before, more likely to be interested
                score += Math.Min(0.3, pastSpeedDatingParticipation * 0.1);
            }

            // 5. Recency boost
            if (speedDate.StartTime < DateTime.Now.AddDays(3))
            {
                score += 0.4; // Boost upcoming speed dates
            }
            else if (speedDate.StartTime < DateTime.Now.AddDays(7))
            {
                score += 0.2; // Smaller boost for speed dates within a week
            }

            return score;
        }

        public double CalculateUserMatchScore(User targetUser, User currentUser, List<UserInteraction> pastInteractions)
        {
            double score = 0;

            // 1. Age compatibility
            int currentUserAge = CalculateAge(currentUser.UserProfile.DOB);
            int targetUserAge = CalculateAge(targetUser.UserProfile.DOB);

            // Higher score for closer age
            double ageDifference = Math.Abs(currentUserAge - targetUserAge);
            score += Math.Max(0, 1 - (ageDifference / 10)); // 0-1 score, higher for closer ages

            // 2. Location proximity
            if (currentUser.UserProfile?.Location != null && targetUser.UserProfile?.Location != null)
            {
                double distance = CalculateDistance(
                    currentUser.UserProfile.Location.Lat, currentUser.UserProfile.Location.Lng,
                    targetUser.UserProfile.Location.Lat, targetUser.UserProfile.Location.Lng);

                // Higher score for closer users
                score += Math.Max(0, 1 - (distance / 20)); // 0-1 score, higher for closer users
            }

            // 3. Similar interests (passions)
            if (currentUser.UserProfile?.Passions.Count > 0 && targetUser.UserProfile?.Passions.Count > 0)
            {
                // Intersection of passions
                var commonPassions = currentUser.UserProfile.Passions
                    .Intersect(targetUser.UserProfile.Passions)
                    .Count();

                // More common passions = higher score
                score += Math.Min(0.5, commonPassions * 0.1); // Up to 0.5 for passions
            }

            // 4. Sexual orientation compatibility
            if (currentUser.UserProfile?.SexualOrientation != null &&
                targetUser.UserProfile?.SexualOrientation != null &&
                currentUser.UserProfile?.Gender != null &&
                targetUser.UserProfile?.Gender != null)
            {
                bool isCompatible = false;

                string currentUserGender = currentUser.UserProfile.Gender.GenderType.ToLower();
                string currentUserOrientation = currentUser.UserProfile.SexualOrientation.SexualOrientationType.ToLower();

                string targetUserGender = targetUser.UserProfile.Gender.GenderType.ToLower();
                string targetUserOrientation = targetUser.UserProfile.SexualOrientation.SexualOrientationType.ToLower();

                // Check if current user is interested in target user's gender
                bool currentUserInterestedInTarget =
                    (currentUserOrientation == "men" && targetUserGender == "male") ||
                    (currentUserOrientation == "women" && targetUserGender == "female") ||
                    (currentUserOrientation == "both");

                // Check if target user is interested in current user's gender
                bool targetUserInterestedInCurrent =
                    (targetUserOrientation == "men" && currentUserGender == "male") ||
                    (targetUserOrientation == "women" && currentUserGender == "female") ||
                    (targetUserOrientation == "both");

                // Both users must be interested in each other's genders
                isCompatible = currentUserInterestedInTarget && targetUserInterestedInCurrent;

                if (isCompatible)
                {
                    score += 0.5;
                    _logger.LogInfo("Sexual orientation compatibility: +0.5");
                }
            }

            // 5. Past interactions with similar users
            // This would compare profiles of past matches and find similarities

            return score;
        }

        private int CalculateAge(DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula implementation
            double R = 6371; // Earth radius in km

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Distance in km
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        private double CalculateEventSimilarity(Events event1, Events event2)
        {
            double similarity = 0;

            // Basic property comparison
            if (event1.EventType == event2.EventType) similarity += 0.3;
            if (event1.EventCategory == event2.EventCategory) similarity += 0.2;

            // Location similarity
            if (event1.Location != null && event2.Location != null)
            {
                double distance = CalculateDistance(
                    event1.Location.Lat, event1.Location.Lng,
                    event2.Location.Lat, event2.Location.Lng);

                similarity += Math.Max(0, 5 - distance) / 10;
            }

            return similarity;
        }
    }
}
