

using Fliq.Domain.Entities.PlatformCompliance;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IComplianceRepository
    {
        Task AddAsync(Compliance compliance);
        Task UpdateAsync(Compliance compliance);
    }
}
