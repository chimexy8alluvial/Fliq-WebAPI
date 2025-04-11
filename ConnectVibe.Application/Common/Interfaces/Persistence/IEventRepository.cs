using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IEventRepository
    {
        void Add(Events events);

        Events? GetEventById(int Id);
        Task<Events?> GetEventByIdAsync(int Id);

        void Update(Events request);

        List<Events> GetAllEvents();
        Task<IEnumerable<GetEventsResult>> GetAllEventsForDashBoardAsync(GetEventsListRequest query);
        Task<IEnumerable<GetEventsResult>> GetAllFlaggedEventsForDashBoardAsync(GetEventsListRequest query);
         

        //Count Queries
        Task<int> CountAllEvents();
        Task<int> CountAllEventsWithPendingApproval();
        Task<int> CountAllSponsoredEvents();
    }
}