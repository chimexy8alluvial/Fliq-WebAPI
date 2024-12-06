using Fliq.Domain.Entities;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IPaymentRepository
    {
        void Add(Payment payment);

        void Update(Payment payment);

        Payment? GetPaymentById(int id);
    }
}