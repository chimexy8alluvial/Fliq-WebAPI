using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.Profile;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class BusinessIdentificationDocumentRepository : IBusinessIdentificationDocumentRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly ILoggerManager _logger;
        public BusinessIdentificationDocumentRepository(FliqDbContext dbContext, ILoggerManager logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public void Add(BusinessIdentificationDocument businessDocument)
        {
            if (businessDocument == null)
            {
                throw new ArgumentNullException(nameof(businessDocument));
            }

            _logger.LogInfo($"Adding business identification document for user Profile ID: {businessDocument.UserProfileId}, document type ID: {businessDocument.BusinessIdentificationDocumentTypeId}");
            _dbContext.BusinessIdentificationDocuments.Add(businessDocument);
        }

        public void Delete(int id)
        {
            var businessDocument = _dbContext.BusinessIdentificationDocuments.Find(id);
            if (businessDocument != null)
            {
                _logger.LogInfo($"Deleting business document ID: {id}");
                _dbContext.BusinessIdentificationDocuments.Remove(businessDocument);
            }

        }

        public async Task<BusinessIdentificationDocument> GetByIdAsync(int id)
        {
            return await _dbContext.BusinessIdentificationDocuments.Include(bd => bd.BusinessIdentificationDocumentType)
                .FirstOrDefaultAsync(bd => bd.Id == id);
        }

        public void Update(BusinessIdentificationDocument businessDocument)
        {
            // No need to call Update if entity is already being tracked
            // EF Core will handle this, but we can explicitly set state if needed
            _logger.LogInfo($"Updating business document ID: {businessDocument.Id}");
            _dbContext.Entry(businessDocument).State = EntityState.Modified;
        }
    }
}
