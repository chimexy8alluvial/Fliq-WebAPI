

using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.PlatformCompliance
{
    public class Compliance : Record
    {
        public int ComplianceTypeId { get; set; }
        public ComplianceType ComplianceType { get; set; } = default!;
        public Language Language { get; set; }
        public string Description { get; set; } = default!;
        public string VersionNumber { get; set; } = default!;
        public DateTime EffectiveDate { get; set; } = default!;
    }
}
