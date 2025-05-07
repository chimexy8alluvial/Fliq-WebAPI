
namespace Fliq.Contracts.PlatformCompliance
{
    public record CreateComplianceRequest(int ComplianceTypeId,
    int Language,
    string Description,
    string VersionNumber,
    DateTime EffectiveDate
    );
  
}
