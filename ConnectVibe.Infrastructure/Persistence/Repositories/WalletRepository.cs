using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public WalletRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public Wallet Add(Wallet wallet)
        {
            _dbContext.Wallets.Add(wallet);
            _dbContext.SaveChanges();
            return wallet;
        }

        public Wallet? GetWalletByUserId(int userId)
        {
            return _dbContext.Wallets.FirstOrDefault(w => w.UserId == userId);
        }

        public bool UpdateWallet(Wallet wallet)
        {
            _dbContext.Update(wallet);
            _dbContext.SaveChanges();
            return true;
        }
    }
}