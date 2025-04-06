using Fliq.Application.AuditTrail.Common;


namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IAuditTrailRepository
    {
        Task AddAuditTrailAsync(Fliq.Domain.Entities.AuditTrail auditTrail);
        Task<(List<AuditTrailListItem> List, int TotalCount)> GetAllAuditTrailsAsync(int PageNumber, int PageSize, string? Name);
    }
}
