using Fliq.Application.Event.Commands.Create;
using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IEventDetailsRepository
    {
        void Add(CreateEventDetailsCommand eventsDetails);
        //EventsDetails? GetEventDetailsById(int id);
    }
}
