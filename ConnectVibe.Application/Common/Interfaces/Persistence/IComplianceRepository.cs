

using Fliq.Domain.Entities.PlatformCompliance;
using Fliq.Domain.Enums;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IComplianceRepository
    {
        Task AddAsync(Compliance compliance);
        Task UpdateAsync(Compliance compliance);
        Task<Compliance?> GetByIdAsync(int id);
        Task<Compliance?> GetLatestComplianceByTypeAsync(int complianceTypeId);
        Task<List<Compliance>> GetAllCompliancesByTypeAsync(int complianceTypeId);
    }
}
