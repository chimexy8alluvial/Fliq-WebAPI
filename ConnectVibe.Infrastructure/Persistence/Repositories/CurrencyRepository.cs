using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Event;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public CurrencyRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(Fliq.Domain.Entities.Event.Currency currency)
        {
            if (currency.Id > 0)
            {
                _dbContext.Update(currency);
            }
            else
            {
                _dbContext.Add(currency);
            }
            _dbContext.SaveChanges();
        }

        public void Update(Fliq.Domain.Entities.Event.Currency currency)
        {
            _dbContext.Update(currency);
            _dbContext.SaveChanges();
        }

        public List<Fliq.Domain.Entities.Event.Currency> GetCurrencies()
        {
            var result = _dbContext.Currencies.ToList();
            return result;
        }

        public Fliq.Domain.Entities.Event.Currency? GetCurrencyById(int id)
        {
            var result = _dbContext.Currencies.SingleOrDefault(p => p.Id == id);
            return result;
        }

        public Fliq.Domain.Entities.Event.Currency GetCurrencyByCode(string code)
        {
            var result = _dbContext.Currencies.SingleOrDefault(p => p.CurrencyCode == code);
            return result;
        }
    }
}