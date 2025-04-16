
namespace Fliq.Domain.Entities.PlatformCompliance
{
    public class ComplianceType : Record
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        
        // Navigation property for related compliances
        public virtual ICollection<Compliance> Compliances { get; set; } = new List<Compliance>();
    }
}
