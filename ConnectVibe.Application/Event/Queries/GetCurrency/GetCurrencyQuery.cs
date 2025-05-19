using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using MediatR;

namespace Fliq.Application.Event.Queries.GetCurrency
{
    public record GetCurrenciesQuery() : IRequest<ErrorOr<List<Fliq.Domain.Entities.Event.Currency>>>;

    public class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, ErrorOr<List<Fliq.Domain.Entities.Event.Currency>>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILoggerManager _logger;

        public GetCurrenciesQueryHandler(ITicketRepository ticketRepository, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<Fliq.Domain.Entities.Event.Currency>>> Handle(GetCurrenciesQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Get Currencies query was called.");
            var currencies = _ticketRepository.GetCurrenciees();
            return currencies;
        }
    }
}