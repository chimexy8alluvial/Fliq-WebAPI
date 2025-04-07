using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.PlatformCompliance;

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
    }
}
