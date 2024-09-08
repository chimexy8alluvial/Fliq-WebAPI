using ConnectVibe.Domain.Entities;
using ConnectVibe.Domain.Entities;
using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IEventRepository
    {
        void Add(Events events);
        User? GetUserById(int Id);
    }
}
