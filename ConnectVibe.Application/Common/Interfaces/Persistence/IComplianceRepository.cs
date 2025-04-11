

using Fliq.Domain.Entities.PlatformCompliance;
using Fliq.Domain.Enums;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IComplianceRepository
    {
        Task AddAsync(Compliance compliance);
        Task UpdateAsync(Compliance compliance);
        Task<Compliance?> GetByIdAsync(int id);
        Task<Compliance?> GetLatestComplianceByTypeAsync(ComplianceType complianceType);
        Task<List<Compliance>> GetAllCompliancesByTypeAsync(ComplianceType complianceType);
    }
}
