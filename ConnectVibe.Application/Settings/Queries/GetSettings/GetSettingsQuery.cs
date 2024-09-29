using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Settings.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Fliq.Domain.Common.Errors;
using System.Security.Claims;

namespace Fliq.Application.Settings.Queries.GetSettings
{
    public record GetSettingsQuery() : IRequest<ErrorOr<GetSettingsResult>>;

    public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, ErrorOr<GetSettingsResult>>
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int UnauthorizedUserId = -1;

        public GetSettingsQueryHandler(ISettingsRepository settingsRepository, IProfileRepository profileRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _settingsRepository = settingsRepository;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ErrorOr<GetSettingsResult>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var userId = GetUserId();

            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return Errors.User.UserNotFound;
            }

            var settings = _settingsRepository.GetSettingByUserId(userId);
            if (settings == null)
            {
                return Errors.Settings.SettingsNotFound;
            }

            return new GetSettingsResult(
              settings.Id,
              settings.ScreenMode,
              settings.RelationAvailability,
              settings.ShowMusicAndGameStatus,
              settings.Language,
              settings.NotificationPreferences.ToList(),
              user.FirstName + " " + user.LastName,
              user.Email,
              user.Id
              );
        }

        private int GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : UnauthorizedUserId;
        }
    }
}