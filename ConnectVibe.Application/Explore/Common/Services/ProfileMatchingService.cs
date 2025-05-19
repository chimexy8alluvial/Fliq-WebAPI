using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Queries;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using Newtonsoft.Json;

namespace Fliq.Application.Explore.Common.Services
{
    public class ProfileMatchingService : IProfileMatchingService
    {
        private readonly IProfileRepository _profileRepository;
        public ProfileMatchingService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<PaginationResponse<UserProfile>> GetMatchedProfilesAsync(User user, ExploreQuery query)
        {
            var userProfileTypes = JsonConvert.DeserializeObject<List<ProfileType>>(user.UserProfile?.ProfileTypeJson ?? "[]") ?? new List<ProfileType>();
            return await Task.FromResult(_profileRepository.GetMatchedUserProfiles(
                user.Id,
                userProfileTypes,
                query.FilterByDating,
                query.FilterByFriendship,
                query.FilterByEvent,
                query.PaginationRequest));
        }

    }
}
