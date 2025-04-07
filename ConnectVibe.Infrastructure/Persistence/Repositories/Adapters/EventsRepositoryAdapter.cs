using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Event;

namespace Fliq.Infrastructure.Persistence.Repositories.Adapters
{
    public class EventsRepositoryAdapter : IGenericRepository<Events>
    {
        private readonly IEventRepository _eventRepository;

        public EventsRepositoryAdapter(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Events?> GetByIdAsync(int id)
        {
            await Task.CompletedTask;
            return _eventRepository.GetEventById(id);
        }

        public async Task UpdateAsync(Events events)
        {
            await Task.CompletedTask;
            _eventRepository.Update(events);
        }

        public async Task<int> CountAsync()
        {
            return await _eventRepository.CountAsync();
        }

        public async Task<int> FlaggedCountAsync()
        {
            return await _eventRepository.FlaggedCountAsync();
        }
    }
}
