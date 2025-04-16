using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IEventRepository
    {
        void Add(Events events);

        Events? GetEventById(int Id);

        void Update(Events request);

        List<Events> GetAllEvents();
        Task<IEnumerable<GetEventsResult>> GetAllEventsForDashBoardAsync(GetEventsListRequest query);
        Task<IEnumerable<GetEventsResult>> GetAllFlaggedEventsForDashBoardAsync(GetEventsListRequest query);

        Task<(IEnumerable<Events> Data, int TotalCount)> GetEventsAsync(
           LocationDetail? userLocation,
        double? maxDistanceKm,
        UserProfile? userProfile,
        EventCategory? category,
        EventType? eventType,
        int? creatorId,
        EventStatus? status, 
        PaginationRequest pagination
        );


        //Count Queries
        Task<int> CountAllEvents();
        Task<int> CountAllEventsWithPendingApproval();
        Task<int> CountAllSponsoredEvents();
    }
}