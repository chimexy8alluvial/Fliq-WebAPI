

using Fliq.Application.DashBoard.Common.UnifiedSearch;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface ISearchRepository
    {
        Task<UnifiedSearchResult> SearchAcrossEntitiesAsync(string searchTerm);
    }
}
