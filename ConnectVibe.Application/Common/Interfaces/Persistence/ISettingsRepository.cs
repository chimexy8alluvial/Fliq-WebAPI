using Fliq.Domain.Entities.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface ISettingsRepository
    {
        void Add(Setting setting);

        void Update(Setting setting);

        Setting? GetSettingById(int id);
    }
}