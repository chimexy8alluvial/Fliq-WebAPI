using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Infrastructure.Persistence;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly ConnectVibeDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public SettingsRepository(ConnectVibeDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(Setting setting)
        {
            if (setting.Id > 0)
            {
                _dbContext.Update(setting);
            }
            else
            {
                _dbContext.Add(setting);
            }
            _dbContext.SaveChanges();
        }

        public void Update(Setting setting)
        {
            _dbContext.Update(setting);

            _dbContext.SaveChanges();
        }

        public Setting? GetSettingById(int id)
        {
            var setting = _dbContext.Settings.SingleOrDefault(p => p.Id == id);
            return setting;
        }
    }
}