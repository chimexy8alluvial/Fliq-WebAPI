using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Infrastructure.Persistence;

using Fliq.Application.Common.Interfaces.Persistence;

using Fliq.Domain.Entities;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ConnectVibeDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public PaymentRepository(ConnectVibeDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(Payment payment)
        {
            if (payment.Id > 0)
            {
                _dbContext.Update(payment);
            }
            else
            {
                _dbContext.Add(payment);
            }
            _dbContext.SaveChanges();
        }

        public void Update(Payment payment)
        {
            _dbContext.Update(payment);

            _dbContext.SaveChanges();
        }
    }
}