using Fliq.Application.DatingEnvironment.Common;
using Fliq.Contracts.Dating;
using Fliq.Domain.Entities.DatingEnvironment;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IBlindDateRepository
    {
        Task<BlindDate?> GetByIdAsync(int id);
        Task<IEnumerable<BlindDate>> GetAllAsync();
        Task<IEnumerable<BlindDate>> GetByCategoryAsync(int categoryId);
        Task AddAsync(BlindDate blindDate);
        Task UpdateAsync(BlindDate blindDate);
        Task DeleteAsync(BlindDate blindDate);
        Task<int> GetBlindDateCountAsync();
        Task<int> DeleteMultipleAsync(List<int> blindDatingId);
        Task<(List<DatingListItems> List, int blindCount)> GetAllFilteredListAsync(string Title, DatingType? Type, TimeSpan? Duration, string SubscriptionType, DateTime? dateCreatedFrom, DateTime? dateCreatedTo, string CreatedBy, int Page, int PageSize);
    }
}
