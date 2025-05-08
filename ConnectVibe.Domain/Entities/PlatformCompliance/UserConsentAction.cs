
namespace Fliq.Domain.Entities.PlatformCompliance
{
    public class UserConsentAction : Record
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public string IPAddress { get; set; } = default!;
        public bool OptIn { get; set; }

        public int ComplianceId { get; set; }
        public Compliance Compliance { get; set; } = default!;
    }
}
