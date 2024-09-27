namespace Fliq.Infrastructure.Authentication
{
    public class GoogleAuthSettings
    {
        public static string SectionName = "GoogleAuthSettings";

        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;

        public string LocationApiKey { get; set; } = string.Empty;
        public string LocationApiEndpoint { get; set; } = string.Empty;
    }
}