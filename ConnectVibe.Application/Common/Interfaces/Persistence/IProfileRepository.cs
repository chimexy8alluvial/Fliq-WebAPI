﻿using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IProfileRepository
    {
        void Add(UserProfile userProfile);

        void Update(UserProfile profile);

        UserProfile? GetUserProfileByUserId(int id);

        IEnumerable<UserProfile> GetMatchedUserProfiles(int userId, List<ProfileType> userProfileTypes, bool? filterByDating, bool? filterByFriendship, PaginationRequest paginationRequest);

        UserProfile? GetProfileByUserId(int id);
    }
}