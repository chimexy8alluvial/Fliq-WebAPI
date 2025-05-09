﻿using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DatingEnvironment.Queries.DatingDashboard.BlindDte
{
    public record BlindDateCountQuery() : IRequest<ErrorOr<CountResult>>;
    public class BlindDateCountQueryHandler : IRequestHandler<BlindDateCountQuery, ErrorOr<CountResult>>
    {
        private readonly ILoggerManager _logger;
        private readonly IBlindDateRepository _blindDateRepository;
        public BlindDateCountQueryHandler(ILoggerManager logger, IBlindDateRepository blindDateRepository)
        {
            _blindDateRepository = blindDateRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(BlindDateCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all blind date count...");

            var count = await _blindDateRepository.GetBlindDateCountAsync();
            _logger.LogInfo($"All blind date count: {count}");

            return new CountResult(count);
        }
    }
}





