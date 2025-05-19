using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface ICurrencyRepository
    {
        void Add(Fliq.Domain.Entities.Event.Currency currency);

        void Update(Fliq.Domain.Entities.Event.Currency currency);

        List<Fliq.Domain.Entities.Event.Currency> GetCurrencies();

        Fliq.Domain.Entities.Event.Currency? GetCurrencyById(int id);

        Fliq.Domain.Entities.Event.Currency GetCurrencyByCode(string code);
    }
}