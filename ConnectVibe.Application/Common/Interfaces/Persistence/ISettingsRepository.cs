using Fliq.Domain.Entities.Settings;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface ISettingsRepository
    {
        void Add(Setting setting);

        void Update(Setting setting);

        Setting? GetSettingById(int id);

        Setting? GetSettingByUserId(int id);
    }
}