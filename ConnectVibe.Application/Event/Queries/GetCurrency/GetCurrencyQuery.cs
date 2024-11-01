using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Event.Queries.GetCurrency
{
    public record GetCurrenciesQuery() : IRequest<ErrorOr<List<Currency>>>;

    public class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, ErrorOr<List<Currency>>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public GetCurrenciesQueryHandler(ITicketRepository ticketRepository, IMapper mapper, ILoggerManager logger)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ErrorOr<List<Currency>>> Handle(GetCurrenciesQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Get Currencies query was called.");
            var currencies = _ticketRepository.GetCurrenciees();
            return currencies;
        }
    }
}