using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.PlatformCompliance;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class ComplianceTypeRepository : IComplianceTypeRepository
    {
        private readonly FliqDbContext _dbContext;

        public ComplianceTypeRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        public async Task AddAsync(ComplianceType ComplianceType)
        {
            if (ComplianceType.Id > 0)
            {
                _dbContext.ComplianceTypes.Update(ComplianceType);
            }
            else
            {
                await _dbContext.ComplianceTypes.AddAsync(ComplianceType);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(ComplianceType ComplianceType)
        {
            _dbContext.ComplianceTypes.Update(ComplianceType);
            await _dbContext.SaveChangesAsync();
        }

       public async Task<ComplianceType?> GetByIdAsync(int id)
       {
            var complianceType = await _dbContext.ComplianceTypes
                .Where(c => c.Id == id).FirstOrDefaultAsync();
            return complianceType;
       }
    }
}
