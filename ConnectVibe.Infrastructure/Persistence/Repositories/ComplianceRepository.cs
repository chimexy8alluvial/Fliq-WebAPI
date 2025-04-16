using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.PlatformCompliance;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class ComplianceRepository : IComplianceRepository
    {

        private readonly FliqDbContext _dbContext;


        public ComplianceRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Compliance compliance)
        {
            if (compliance.Id > 0)
            {
                _dbContext.Compliances.Update(compliance);
            }
            else
            {
                await _dbContext.Compliances.AddAsync(compliance);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Compliance compliance)
        {
            _dbContext.Compliances.Update(compliance);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Compliance?> GetByIdAsync(int id)
        {
            return await _dbContext.Compliances
            .Where(c => !c.IsDeleted && c.Id == id)
            .FirstOrDefaultAsync();
        }

        public async Task<Compliance?> GetLatestComplianceByTypeAsync(int complianceTypeId)
        {
            return await _dbContext.Compliances
                .Where(c => !c.IsDeleted && c.ComplianceTypeId == complianceTypeId)
                .OrderByDescending(c => c.EffectiveDate)
                .ThenByDescending(c => c.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Compliance>> GetAllCompliancesByTypeAsync(int complianceTypeId)
        {
            return await _dbContext.Compliances
                .Where(c => !c.IsDeleted && c.ComplianceTypeId == complianceTypeId)
                .OrderByDescending(c => c.EffectiveDate)
                .ToListAsync();
        }
    }
}
