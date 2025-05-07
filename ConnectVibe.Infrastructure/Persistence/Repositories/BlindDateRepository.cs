

using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.DatingEnvironment;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.Event.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class BlindDateRepository : IBlindDateRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public BlindDateRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public async Task<BlindDate?> GetByIdAsync(int id)
        {
            return await _dbContext.BlindDates
                .Where(b => !b.IsDeleted)
                .Include(b => b.BlindDateCategory)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BlindDate>> GetAllAsync()
        {
            return await _dbContext.BlindDates
                .Where(b => !b.IsDeleted)
                .Include(b => b.BlindDateCategory)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlindDate>> GetByCategoryAsync(int categoryId)
        {
            return await _dbContext.BlindDates
                .Where(b => b.CategoryId == categoryId && !b.IsDeleted )
                .Include(b => b.BlindDateCategory)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlindDate>> GetBlindDatesForAdmin(int pageSize, int pageNumber, int? creationStatus)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sp_BlindDateListForAdmin";

                var parameter = new { 
                    PageSize = pageSize,
                  PageNumber = pageNumber,
                  CreationStatus = creationStatus
                };

                var blindDates = await connection.QueryAsync<BlindDate>(sql, parameter, commandType: CommandType.StoredProcedure);
                return blindDates.ToList();
            }

        }

        public async Task AddAsync(BlindDate blindDate)
        {
            if(blindDate.Id > 0)
            {
                _dbContext.BlindDates.Update(blindDate);
            }
            else
            {
                await _dbContext.BlindDates.AddAsync(blindDate);
            }
   
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(BlindDate blindDate)
        {
            _dbContext.BlindDates.Update(blindDate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(BlindDate blindDate)
        {
            blindDate.IsDeleted = true;
            _dbContext.BlindDates.Update(blindDate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {

            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountBlindDates", commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> FlaggedCountAsync()
        {

            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountFlaggedEvents", commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> GetBlindDateCountAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_BlindDatingCount", commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> DeleteMultipleAsync(List<int> blindDatingId)
        {
            using (var connection = _connectionFactory.CreateConnection() as DbConnection ?? throw new InvalidOperationException("Connection must be a DbConnection"))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var blindDatingEventIdsTable = new DataTable();
                        blindDatingEventIdsTable.Columns.Add("Id", typeof(int));
                        foreach (var id in blindDatingId)
                        {
                            blindDatingEventIdsTable.Rows.Add(id);
                        }

                        var parameters = new
                        {
                            BlindDateIds = blindDatingEventIdsTable.AsTableValuedParameter("dbo.BlindDatesIdTableType")
                        };

                        var deletedCount = await connection.ExecuteScalarAsync<int>(
                            "sp_DeleteBlindDatingEvents", parameters, commandType: CommandType.StoredProcedure,
                            transaction: transaction
                        );

                        transaction.Commit();
                        return deletedCount;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<(List<DatingListItems> List, int blindCount)> GetAllFilteredListAsync(string title, DatingType? type, TimeSpan? duration, string subscriptionType, DateTime? dateCreatedFrom, DateTime? dateCreatedTo, string createdBy, int page, int pageSize)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {

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

                using (var multi = await connection.QueryMultipleAsync("sp_GetAllFilteredBlindDatingList", parameters, commandType: CommandType.StoredProcedure))
                {
                    var list = (await multi.ReadAsync<DatingListItems>()).AsList();
                    var totalCount = await multi.ReadSingleAsync<int>();
                    return (list, totalCount);
                }
            }
        }
    }
}