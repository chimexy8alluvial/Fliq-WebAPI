using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IEventRepository
    {
        void Add(Events events);

        Events? GetEventById(int Id);

        void Update(Events request);

        List<Events> GetAllEvents();
    }
}