using Fliq.Contracts.Contents;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IContentRepository
    {
        Task<IEnumerable<ContentTypeCount>> GetAllContentCountsAsync();
        Task<int> GetTotalContentCountAsync();
    }
}
