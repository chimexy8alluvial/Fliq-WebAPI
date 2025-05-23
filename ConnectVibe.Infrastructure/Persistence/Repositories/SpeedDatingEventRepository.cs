using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.DatingEnvironment.Common;
using Fliq.Contracts.Dating;
using Fliq.Domain.Entities.DatingEnvironment;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class SpeedDatingEventRepository : ISpeedDatingEventRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public SpeedDatingEventRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public async Task<SpeedDatingEvent?> GetByIdAsync(int id)
        {
            return await _dbContext.SpeedDatingEvents
                .Where(b => !b.IsDeleted)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<SpeedDatingEvent>> GetAllAsync()
        {
            return await _dbContext.SpeedDatingEvents
                .Where(b => !b.IsDeleted)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .ToListAsync();
        }

        public async Task<IEnumerable<SpeedDatingEvent>> GetByCategoryAsync(SpeedDatingCategory category)
        {
            return await _dbContext.SpeedDatingEvents
                .Where(b => b.Category == category && !b.IsDeleted)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .ToListAsync();
        }

        public async Task AddAsync(SpeedDatingEvent speedDating)
        {
            if (speedDating.Id > 0)
            {
                _dbContext.SpeedDatingEvents.Update(speedDating);
            }
            else
            {
                await _dbContext.SpeedDatingEvents.AddAsync(speedDating);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(SpeedDatingEvent speedDate)
        {
            _dbContext.SpeedDatingEvents.Update(speedDate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(SpeedDatingEvent speedDate)
        {
            speedDate.IsDeleted = true;
            _dbContext.SpeedDatingEvents.Update(speedDate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetSpeedDateCountAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_SpeedDatingCount", commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> DeleteMultipleAsync(List<int> speedDatingId)
        {
            using (var connection = _connectionFactory.CreateConnection() as DbConnection ?? throw new InvalidOperationException("Connection must be a DbConnection"))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var speedDatingEventIdsTable = new DataTable();
                        speedDatingEventIdsTable.Columns.Add("Id", typeof(int));
                        foreach (var id in speedDatingId)
                        {
                            speedDatingEventIdsTable.Rows.Add(id);
                        }

                        var parameters = new
                        {
                            SpeedDateIds = speedDatingEventIdsTable.AsTableValuedParameter("dbo.SpeedDatesIdTableType")
                        };

                        var deletedCount = await connection.ExecuteScalarAsync<int>(
                            "sp_DeleteSpeedDatingEvents", parameters, commandType: CommandType.StoredProcedure,
                            transaction: transaction
                        );

                        transaction.Commit();
                        return deletedCount;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw; // Or handle/log the exception as needed
                    }
                }

            }
        }

        public async Task<(List<DatingListItems> List, int speedCount)> GetAllFilteredListAsync(string title, DatingType? type, TimeSpan? duration, string subscriptionType, DateTime? dateCreatedFrom, DateTime? dateCreatedTo, string createdBy, int page, int pageSize)
        {
            using (var connection = _connectionFactory.CreateConnection() as DbConnection ?? throw new InvalidOperationException("Connection must be a DbConnection"))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    Page = page,
                    PageSize = pageSize,
                    Title = title,
                    Type = type.HasValue ? (int)type.Value : (int?)null,
                    CreatedBy = createdBy,
                    SubscriptionType = subscriptionType,
                    Duration = duration,
                    DateCreatedFrom = dateCreatedFrom,
                    DateCreatedTo = dateCreatedTo
                };

                using (var multi = await connection.QueryMultipleAsync("sp_GetAllFilteredSpeedDatingList", parameters, commandType: CommandType.StoredProcedure))
                {
                    var list = (await multi.ReadAsync<DatingListItems>()).AsList();
                    var totalCount = await multi.ReadSingleAsync<int>();
                    return (list, totalCount);
                }
            }
        }

        public async Task<IEnumerable<SpeedDatingEvent>> GetUpcomingSpeedDatingEvents()
        {
            return await _dbContext.SpeedDatingEvents
                .Where(b => b.StartTime > DateTime.UtcNow && !b.IsDeleted)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .ToListAsync();
        }
    }
}
