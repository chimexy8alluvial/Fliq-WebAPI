using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    
    public class AuditTrailRepository : IAuditTrailRepository
    {
        private readonly FliqDbContext _dbContext;
        public AuditTrailRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAuditTrailAsync(AuditTrail auditTrail)
        {
            if (auditTrail == null)
                throw new ArgumentNullException(nameof(auditTrail));

            auditTrail.IPAddress = auditTrail.IPAddress ?? string.Empty;

            _dbContext.Set<AuditTrail>().Add(auditTrail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<AuditTrail>> GetAllAuditTrailsAsync(string? filterByAction = null, string? filterByUserEmail = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.Set<AuditTrail>().AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filterByAction))
            {
                query = query.Where(a => a.AuditAction.Contains(filterByAction));
            }

            if (!string.IsNullOrEmpty(filterByUserEmail))
            {
                query = query.Where(a => a.UserEmail.Contains(filterByUserEmail));
            }

            //if (startDate.HasValue)
            //{
            //    query = query.Where(a => a.CreatedAt >= startDate.Value);
            //}

            //if (endDate.HasValue)
            //{
            //    query = query.Where(a => a.CreatedAt <= endDate.Value);
            //}

            // Execute stored procedure for final retrieval (example)
            var results = await query.ToListAsync();

            return results;
        }
    }
}
