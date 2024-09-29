using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Explore.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Explore.Queries
{
    public record ExploreQuery(
        bool? FilterByEvent = null,
        bool? FilterByDating = null,
        bool? FilterByFriendship = null
        ) : IRequest<ErrorOr<ExploreResult>>;

    public class ExploreQueryHandler : IRequestHandler<ExploreQuery, ErrorOr<ExploreResult>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private const int UnauthorizedUserId = -1;
        //private readonly IEventRepository _eventRepository;

        public ExploreQueryHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IProfileRepository profileRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _profileRepository = profileRepository;
        }

        public async Task<ErrorOr<ExploreResult>> Handle(ExploreQuery query, CancellationToken cancellationToken)
        {
            // Get logged-in user
            var user = GetUser();
            if(user == null)
            {
                return Errors.User.UserNotFound;
            }

            if(user.UserProfile == null)
            {
                return Errors.Profile.ProfileNotFound;
            }

            // Fetch user profiles based on filters
            var profiles = await _profileRepository.GetProfilesAsync(user.Id, query.FilterByDating, query.FilterByFriendship);

            //Fetch events if the user is exploring events
            #region Fetch Events
            
            //IEnumerable<Event> events = new();
            if (user.UserProfile.ProfileTypes.Contains(ProfileType.Events))
            {
                //if(query.FilterByEvent == true)
                //{
                //    events = await _eventRepository.GetEventsAsync();
                //}
            }
            #endregion

            return new ExploreResult(profiles);
        }

        private int GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : UnauthorizedUserId;
        }

        private User? GetUser()
        {
            var userId = GetUserId();
            var user = _userRepository.GetUserById(userId);
            return user;
        }
    }

    
}
