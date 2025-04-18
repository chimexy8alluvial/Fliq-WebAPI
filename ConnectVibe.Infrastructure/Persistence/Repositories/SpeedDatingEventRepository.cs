﻿using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class SpeedDatingEventRepository : ISpeedDatingEventRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public SpeedDatingEventRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public async Task<SpeedDatingEvent?> GetByIdAsync(int id)
        {
            return await _dbContext.SpeedDatingEvents
                .Where(b => !b.IsDeleted)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<SpeedDatingEvent>> GetAllAsync()
        {
            return await _dbContext.SpeedDatingEvents
                .Where(b => !b.IsDeleted)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .ToListAsync();
        }

        public async Task<IEnumerable<SpeedDatingEvent>> GetByCategoryAsync(SpeedDatingCategory category)
        {
            return await _dbContext.SpeedDatingEvents
                .Where(b => b.Category == category && !b.IsDeleted)
                .Include(b => b.Location)
                .Include(b => b.Participants)
                .ToListAsync();
        }

        public async Task AddAsync(SpeedDatingEvent speedDating)
        {
            if (speedDating.Id > 0)
            {
                _dbContext.SpeedDatingEvents.Update(speedDating);
            }
            else
            {
                await _dbContext.SpeedDatingEvents.AddAsync(speedDating);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(SpeedDatingEvent speedDate)
        {
            _dbContext.SpeedDatingEvents.Update(speedDate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(SpeedDatingEvent speedDate)
        {
            speedDate.IsDeleted = true;
            _dbContext.SpeedDatingEvents.Update(speedDate);
            await _dbContext.SaveChangesAsync();
        }
    }
}
