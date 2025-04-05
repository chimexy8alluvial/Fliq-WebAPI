using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities;
using Microsoft.AspNetCore.Connections;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IAuditTrailRepository
    {
        Task AddAuditTrailAsync(AuditTrail auditTrail);
        Task<List<AuditTrail>> GetAllAuditTrailsAsync(PaginationRequest paginationRequest);
        Task<int> GetTotalAuditTrailCountAsync();


    }
}
