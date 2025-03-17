

namespace Fliq.Contracts.Notifications.Helpers
{
    public static class NotificationMessageHelper
    {
        public static string GetGeneralInactivityMessage() =>
        "It's been a while! Come back and see what's new!";

        public static string GetFeatureInactivityMessage(string feature) =>
            feature switch
            {
                "Initiate-MatchRequest" => "Hey! You haven't been sending new match requests. There are new profiles to explore now!",
                "Explore-Profiles" => "It's been a while since you explored new profiles. Check them out today!",
                "Game" => "You haven't played any games recently. Challenge someone and have fun!",
                "Send-Message" => "Your inbox misses you! Catch up with your connections.",
                _ => $"You haven't used the {feature} feature in a while! Come back and check it out!"
            };
    }
}
