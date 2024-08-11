namespace ConnectVibe.Infrastructure.Authentication
{
    public class GoogleAuthSettings
    {
        public static string SectionName = "GoogleAuthSettings";

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }
}