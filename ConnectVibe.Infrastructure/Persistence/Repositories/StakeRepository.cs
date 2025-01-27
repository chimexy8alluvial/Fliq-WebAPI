using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Games;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class StakeRepository : IStakeRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public StakeRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public Stake Add(Stake stake)
        {
            _dbContext.Stakes.Add(stake);
            _dbContext.SaveChanges();
            return stake;
        }

        public Stake? GetStakeById(int id)
        {
            return _dbContext.Stakes.FirstOrDefault(sr => sr.Id == id);
        }

        public Stake? GetStakeByGameSessionId(int id)
        {
            return _dbContext.Stakes.FirstOrDefault(sr => sr.GameSessionId == id);
        }

        public void UpdateStake(Stake stake)
        {
            _dbContext.Stakes.Update(stake);
            _dbContext.SaveChanges();
        }
    }
}