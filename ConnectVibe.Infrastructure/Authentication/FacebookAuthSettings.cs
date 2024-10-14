namespace Fliq.Infrastructure.Authentication
{
    public class FacebookAuthSettings
    {
        public static string SectionName = "FacebookAuthSettings";

        public string TokenValidationUrl { get; set; } = string.Empty;
        public string UserInfoUrl { get; set; } = string.Empty;
        public string AppId { get; set; } = string.Empty;
        public string AppSecret { get; set; } = string.Empty;
    }
}