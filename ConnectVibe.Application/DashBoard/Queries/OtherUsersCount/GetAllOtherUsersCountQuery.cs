﻿using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.FemaleUsersCount;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.DashBoard.Queries.OtherUsersCount
{
    public record GetAllOtherUsersCountQuery() : IRequest<ErrorOr<UserCountResult>>;

    public class GetAllOtherUsersCountQueryHandler : IRequestHandler<GetAllOtherUsersCountQuery, ErrorOr<UserCountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetAllOtherUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<UserCountResult>> Handle(GetAllOtherUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all other-users count...");

            var count = await _userRepository.CountAllOtherUsers();
            _logger.LogInfo($"All other-users count: {count}");

            return new UserCountResult(count);
        }
    }
}
