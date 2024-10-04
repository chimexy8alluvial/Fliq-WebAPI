using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Explore.Queries;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Explore.Common.Services
{
    public class ProfileMatchingService : IProfileMatchingService
    {
        private readonly IProfileRepository _profileRepository;
        public ProfileMatchingService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<IEnumerable<UserProfile>> GetMatchedProfilesAsync(User user, ExploreQuery query)
        {
            // Get user profile types
            var userProfileTypes = user.UserProfile?.ProfileTypes;
            if (userProfileTypes == null) return Enumerable.Empty<UserProfile>();

            // Retrieve user's sexual orientation preference
            var userSexPreferences = user.UserProfile?.SexualOrientation?.SexualOrientationType.ToString();

            // Use repository to fetch profiles based on broad filters (e.g., friendship, dating)
            var profiles = await _profileRepository.GetProfilesByQuery(
                user.Id,
                userProfileTypes,
                query.FilterByFriendship,
                query.FilterByDating
            ).ToListAsync();

            // Apply matching logic based on user's sexual orientation preference or others
            var matchedProfiles = profiles.Where(profile =>
                userSexPreferences == null || profile.SexualOrientation?.SexualOrientationType.ToString() == userSexPreferences
            );

            // Apply pagination
            return matchedProfiles
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);
        }

    }
}
