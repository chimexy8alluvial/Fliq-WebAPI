using Fliq.Application.DatingEnvironment.Common;
using Fliq.Contracts.Dating;
using Fliq.Domain.Entities.DatingEnvironment;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Enums;
using System.Threading.Tasks;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public interface ISpeedDatingEventRepository
    {
        Task<SpeedDatingEvent?> GetByIdAsync(int id);
        Task<IEnumerable<SpeedDatingEvent>> GetAllAsync();
        Task<IEnumerable<SpeedDatingEvent>> GetSpeedDatesForAdmin(int pageSize, int pageNumber, int? creationStatus);
        Task<IEnumerable<SpeedDatingEvent>> GetByCategoryAsync(SpeedDatingCategory category);
        Task AddAsync(SpeedDatingEvent speedDate);
        Task UpdateAsync(SpeedDatingEvent speedDate);
        Task DeleteAsync(SpeedDatingEvent speedDate);
        Task<int> GetSpeedDateCountAsync();
        Task<int> DeleteMultipleAsync(List<int> speedDatingId);
        Task<(List<DatingListItems> List, int speedCount)> GetAllFilteredListAsync(string Title, DatingType? Type, TimeSpan? Duration, string SubscriptionType, DateTime? dateCreatedFrom, DateTime? dateCreatedTo, string CreatedBy, int Page, int PageSize);
        Task<int> CountAsync();
        Task<int> FlaggedCountAsync();
    }
}