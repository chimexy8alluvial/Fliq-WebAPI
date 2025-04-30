using Dapper;
using Fliq.Application.AuditTrail.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
            {
                throw new ArgumentNullException(nameof(auditTrail));
            }

            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new
                {
                    auditTrail.UserId,
                    auditTrail.UserFirstName,
                    auditTrail.UserLastName,
                    auditTrail.UserEmail,
                    auditTrail.AuditAction,
                    auditTrail.UserRole,
                    auditTrail.IPAddress,
                    auditTrail.IsDeleted,
                    auditTrail.DateCreated,
                    auditTrail.DateModified,
                };

                await connection.ExecuteAsync("sp_AddAuditTrail", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<(List<AuditTrailListItem> List, int TotalCount)> GetAllAuditTrailsAsync(int pageNumber, int pageSize, string? name)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    _loggerManager.LogInfo($"Fetching paginated audit trails. Page: {pageNumber}, PageSize: {pageSize}, Name: {name ?? "null"}");

                    var parameters = new
                    {
                        Page = pageNumber,   
                        PageSize = pageSize, 
                        Name = name           
                    };

                    using (var multi = await connection.QueryMultipleAsync("sp_GetAllPaginatedAuditTrails", parameters, commandType: CommandType.StoredProcedure))
                    {
                        var list = (await multi.ReadAsync<AuditTrailListItem>()).AsList();
                        _loggerManager.LogInfo($"Retrieved {list.Count} audit trails: {System.Text.Json.JsonSerializer.Serialize(list)}");
                        var totalCount = await multi.ReadSingleAsync<int>();
                        return (list, totalCount);
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error fetching paginated audit trails: {ex.Message}");
                throw;
            }
        }
    }
}
