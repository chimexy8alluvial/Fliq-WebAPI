using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IEventReviewRepository
    {
        void Add(EventReview eventReview);

        void Update(EventReview request);

        EventReview? GetEventReviewById(int id);
    }
}