using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Common.Services;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Common.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<ErrorOr<ExploreEventsResult>> Handle(ExploreEventsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Starting ExploreQuery handling for user.");

            // Get logged-in user
            var user = _userRepository.GetUserById(query.UserId);
            if (user == null)
            {
                _logger.LogWarn("User not found");
                return Errors.User.UserNotFound;
            }

            if (user.UserProfile == null)
            {
                _logger.LogWarn($"UserProfile not found for user {user.Id}");
                return Errors.Profile.ProfileNotFound;
            }

            // Fetch events for user based on filters
            _logger.LogInfo($"Fetching events for user --> {user.Id}");
            
            return new ExploreEventsResult();
        }
    }
}
