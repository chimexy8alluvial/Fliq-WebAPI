using Fliq.Domain.Entities;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IAuditTrailRepository
    {
        Task AddAuditTrailAsync(AuditTrail auditTrail);
        Task<List<AuditTrail>> GetAllAuditTrailsAsync(string? filterByAction = null, string? filterByUserEmail = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}
