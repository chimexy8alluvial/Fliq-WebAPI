
using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class BusinessIdentificationDocumentTypeRepository : IBusinessIdentificationDocumentTypeRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggerManager _loggerManager;
        public BusinessIdentificationDocumentTypeRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory, ILoggerManager loggerManager)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
            _loggerManager = loggerManager;
        }
        public async Task AddBusinessIdentificationDocumentTypeAsync(BusinessIdentificationDocumentType documentType)
        {
            await _dbContext.BusinessIdentificationDocumentTypes.AddAsync(documentType);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var documentType = await _dbContext.BusinessIdentificationDocumentTypes
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
            if (documentType != null)
            {
                documentType.IsDeleted = true;
                _dbContext.BusinessIdentificationDocumentTypes.Update(documentType);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> DocumentTypeExists(int documentTypeId)
        {
            return await _dbContext.BusinessIdentificationDocumentTypes.AnyAsync(t => t.Id == documentTypeId);
        }

        public IEnumerable<BusinessIdentificationDocumentType> GetAllBusinessIdentificationDocumentTypesAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                _loggerManager.LogInfo($"Fetching Business document types");

                var businessDocuments = connection.Query<BusinessIdentificationDocumentType>("sp_GetAllBusinessIdentificationDocumentTypes", commandType: CommandType.StoredProcedure);
                return businessDocuments;

            }

        }

        public async Task<BusinessIdentificationDocumentType> GetByIdAsync(int id)
        {
            var document = _dbContext.BusinessIdentificationDocumentTypes.SingleOrDefault(p => p.Id == id && !p.IsDeleted);
            return document;
        }


        public async Task<bool> IsInUseAsync(int id)
        {
            return await _dbContext.Users
                   .AnyAsync(u => u.UserProfile.BusinessIdentificationDocument.BusinessIdentificationDocumentTypeId == id);
        }
    }
}
