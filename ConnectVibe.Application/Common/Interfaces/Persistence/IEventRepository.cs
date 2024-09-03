using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IEventRepository
    {
        void Add(CreateEvent createEvent);
        //UserProfile? GetUserById(int Id);
    }
}
