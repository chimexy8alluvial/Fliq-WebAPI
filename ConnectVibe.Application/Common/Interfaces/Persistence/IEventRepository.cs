using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IEventRepository
    {
        void Add(Events events);

        Events? GetEventById(int Id);

        void Update(Events request);

        List<Events> GetAllEvents();
        Task<IEnumerable<Events>> GetAllEventsForDashBoardAsync(GetEventsListRequest query);
        Task<IEnumerable<Events>> GetAllFlaggedEventsForDashBoardAsync(GetEventsListRequest query);
        Task<IEnumerable<Events>> GetAllCancelledEventsForDashBoardAsync(GetEventsListRequest query);
        

        //Count Queries
        Task<int> CountAllEvents();

        Task<int> CountAllSponsoredEvents();
    }
}