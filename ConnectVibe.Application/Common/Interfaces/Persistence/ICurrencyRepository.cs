using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface ICurrencyRepository
    {
        void Add(Currency currency);

        void Update(Currency currency);

        List<Currency> GetCurrencies();

        Currency? GetCurrencyById(int id);

        Currency GetCurrencyByCode(string code);
    }
}