

namespace Fliq.Application.PlatformCompliance.Common
{
    public class UserConsentStatusResult
    {
        public bool HasConsented { get; set; }
        public DateTime? ConsentDate { get; set; }
        public string? VersionNumber { get; set; }
        public bool NeedsReview { get; set; }
    }
}
