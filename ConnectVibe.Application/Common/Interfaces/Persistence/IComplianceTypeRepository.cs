
using Fliq.Domain.Entities.PlatformCompliance;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IComplianceTypeRepository
    {
        Task<ComplianceType?> GetByIdAsync(int id);
        Task AddAsync(ComplianceType complianceType);
        Task UpdateAsync(ComplianceType complianceType);
    }
}
