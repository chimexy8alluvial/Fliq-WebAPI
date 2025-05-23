

namespace Fliq.Contracts.PlatformCompliance
{
    public record RecordUserConsentRequest(
       int ComplianceId,
       bool OptIn
    );
}
