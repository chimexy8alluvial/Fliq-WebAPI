using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using ErrorOr;

using Fliq.Application.Common.Interfaces.Persistence;

using Fliq.Application.Settings.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Settings;
using MediatR;

namespace Fliq.Application.Settings.Commands.Update
{
    public record UpdateSettingsCommand
        (
        int Id,
        ScreenMode ScreenMode,
         bool RelationAvailability,
        bool ShowMusicAndGameStatus,
        string Language,
        List<NotificationPreference> NotificationPreferences,
        int UserId
        ) : IRequest<ErrorOr<GetSettingsResult>>;

    public class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, ErrorOr<GetSettingsResult>>
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public UpdateSettingsCommandHandler(ISettingsRepository settingsRepository, IUserRepository userRepository, ILoggerManager logger)
        {
            _settingsRepository = settingsRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<GetSettingsResult>> Handle(UpdateSettingsCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                return Errors.User.UserNotFound;
            }

            var settings = _settingsRepository.GetSettingById(command.Id);
            if (settings == null)
            {
                return Errors.Settings.SettingsNotFound;
            }

            settings.ScreenMode = command.ScreenMode;
            settings.RelationAvailability = command.RelationAvailability;
            settings.ShowMusicAndGameStatus = command.ShowMusicAndGameStatus;
            settings.Language = command.Language;
            settings.NotificationPreferences = command.NotificationPreferences;

            _settingsRepository.Update(settings);
            _logger.LogInfo($"Settings updated for user {user.Id}");

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
    }
}