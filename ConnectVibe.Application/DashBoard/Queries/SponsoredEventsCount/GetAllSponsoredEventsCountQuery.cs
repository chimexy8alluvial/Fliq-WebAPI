﻿using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.SponsoredEventsCount
{
    public record GetAllSponsoredEventsCountQuery() : IRequest<ErrorOr<CountResult>>;

    public class GetAllSponsoredEventsCountQueryHandler : IRequestHandler<GetAllSponsoredEventsCountQuery, ErrorOr<CountResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetAllSponsoredEventsCountQueryHandler(IEventRepository eventRepository, ILoggerManager logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }
        public async Task<ErrorOr<CountResult>> Handle(GetAllSponsoredEventsCountQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all sponsored-events count...");

            var count = await _eventRepository.CountAllSponsoredEvents();
            _logger.LogInfo($"All sponsored-events count: {count}");

            return new CountResult(count);
        }
    }
}
