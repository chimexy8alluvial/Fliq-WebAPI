using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    
    public class AuditTrailRepository : IAuditTrailRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggerManager _loggerManager;
        public AuditTrailRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory, ILoggerManager loggerManager)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
            _loggerManager = loggerManager;
        }
        public async Task AddAuditTrailAsync(AuditTrail auditTrail)
        {
            if (auditTrail == null)
                throw new ArgumentNullException(nameof(auditTrail));

            auditTrail.IPAddress = auditTrail.IPAddress ?? string.Empty;

            _dbContext.Set<AuditTrail>().Add(auditTrail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<AuditTrail>> GetAllAuditTrailsAsync(PaginationRequest paginationRequest)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    _loggerManager.LogInfo($"Fetching paginated audit trails. Page: {paginationRequest.PageNumber}, PageSize: {paginationRequest.PageSize}");

                    var parameters = CreateDynamicParameters(paginationRequest); ;
                    var sql = "sp_GetAllPaginatedAuditTrails";

                    var auditTrails = await connection.QueryAsync<AuditTrail>(sql, parameters, commandType: CommandType.StoredProcedure);

                    return auditTrails.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error fetching paginated audit trails: {ex.Message}");
                throw;
            }
        }

        private static DynamicParameters CreateDynamicParameters(PaginationRequest paginationRequest)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pageNumber", paginationRequest.PageNumber);
            parameters.Add("@pageSize", paginationRequest.PageSize);

            return parameters;
        }

        public async Task<int> GetTotalAuditTrailCountAsync()
        {
                try
                {
                    using (var connection = _connectionFactory.CreateConnection())
                    {
                        var sql = "SELECT COUNT(*) FROM AuditTrails";

                        var totalCount = await connection.ExecuteScalarAsync<int>(sql);
                        return totalCount;
                    }
                }
                catch (Exception ex)
                {
                    _loggerManager.LogError($"Error fetching total count of audit trails: {ex.Message}");
                    throw;
                }
        }
    }
}
