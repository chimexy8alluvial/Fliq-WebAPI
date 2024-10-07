using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Explore.Queries
{
    public record ExploreEventsQuery(
       int UserId,
        int PageNumber = 1,
        int PageSize = 5
        ) : IRequest<ErrorOr<ExploreEventsResult>>;

    public class ExploreEventsQueryHandler : IRequestHandler<ExploreEventsQuery, ErrorOr<ExploreEventsResult>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ILoggerManager _logger;
        public ExploreEventsQueryHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IProfileRepository profileRepository, ILoggerManager logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _logger = logger;
        }

        public Task<ErrorOr<ExploreEventsResult>> Handle(ExploreEventsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
