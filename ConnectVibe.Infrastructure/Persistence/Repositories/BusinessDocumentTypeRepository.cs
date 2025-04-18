
using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class BusinessDocumentTypeRepository : IBusinessDocumentTypeRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggerManager _loggerManager;
        public BusinessDocumentTypeRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory, ILoggerManager loggerManager)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
            _loggerManager = loggerManager;
        }
        public async Task AddBusinessDocumentTypeAsync(BusinessDocumentType documentType)
        {
            await _dbContext.BusinessDocumentTypes.AddAsync(documentType);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var documentType = await _dbContext.BusinessDocumentTypes
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
            if (documentType != null)
            {
                documentType.IsDeleted = true;
                _dbContext.BusinessDocumentTypes.Update(documentType);
                await _dbContext.SaveChangesAsync();
            }
        }

        public IEnumerable<BusinessDocumentType> GetAllBusinessDocumentTypesAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                _loggerManager.LogInfo($"Fetching Business document types");

                var businessDocuments = connection.Query<BusinessDocumentType>("sp_GetAllBusinessDocumentTypes", commandType: CommandType.StoredProcedure);
                return businessDocuments;

            }

        }

        public async Task<BusinessDocumentType> GetByIdAsync(int id)
        {
            var document = _dbContext.BusinessDocumentTypes.SingleOrDefault(p => p.Id == id && !p.IsDeleted);
            return document;
        }


        public async Task<bool> IsInUseAsync(int id)
        {
            return await _dbContext.Users
                   .AnyAsync(u => u.UserProfile.BusinessIdentificationDocument.BusinessDocumentTypeId == id);
        }
    }
}
