
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.PlatformCompliance;
using Fliq.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class UserConsentRepository : IUserConsentRepository
    {
        private readonly FliqDbContext _dbContext;

        public UserConsentRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserConsentAction?> GetUserConsentForComplianceAsync(int userId, int complianceId)
        {
            return await _dbContext.UserConsentActions
                .Where(u => !u.IsDeleted && u.UserId == userId && u.ComplianceId == complianceId)
                .FirstOrDefaultAsync();
        }

        public async Task<UserConsentAction?> GetUserConsentForComplianceTypeAsync(int userId, int complianceTypeId)
        {
            return await _dbContext.UserConsentActions
                .Where(u => !u.IsDeleted && u.UserId == userId)
                .Include(u => u.Compliance)
                .Where(u => u.Compliance.ComplianceTypeId == complianceTypeId)
                .OrderByDescending(u => u.DateCreated)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserConsentAction>> GetUserConsentsHistoryAsync(int userId, int? complianceTypeId = null)
        {
            // Start with the base query
            IQueryable<UserConsentAction> query = _dbContext.UserConsentActions
                .Where(u => !u.IsDeleted && u.UserId == userId)
                .Include(u => u.Compliance);

            // Apply the compliance type filter if provided
            if (complianceTypeId.HasValue)
            {
                query = query.Where(u => u.Compliance.ComplianceTypeId == complianceTypeId.Value);
            }

            // Execute the query
            return await query
                .OrderByDescending(u => u.DateCreated)
                .ToListAsync();
        }

        public async Task AddAsync(UserConsentAction userConsent)
        {
            await _dbContext.UserConsentActions.AddAsync(userConsent);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserConsentAction userConsent)
        {
            _dbContext.UserConsentActions.Update(userConsent);
            await _dbContext.SaveChangesAsync();
        }
    }
}
