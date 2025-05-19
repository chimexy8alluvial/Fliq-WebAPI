using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IProfileRepository
    {
        void Add(UserProfile userProfile);

        void Update(UserProfile profile);

        UserProfile? GetUserProfileByUserId(int id);

        PaginationResponse<UserProfile> GetMatchedUserProfiles(
               int userId,
               List<ProfileType> userProfileTypes,
               bool? filterByDating,
               bool? filterByFriendship,
               bool? filterByEvent,
               PaginationRequest paginationRequest); UserProfile? GetProfileByUserId(int id);

        Task<string?> GetUserCountryAsync(int userId, CancellationToken cancellationToken);
        Task<Fliq.Domain.Entities.Event.Currency> GetUserCurrencyAsync(int userId, CancellationToken cancellationToken);
    }
}