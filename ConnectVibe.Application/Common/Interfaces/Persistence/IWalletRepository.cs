using Fliq.Domain.Entities;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IWalletRepository
    {
        Wallet Add(Wallet wallet);

        Wallet? GetWalletByUserId(int userId);

        bool UpdateWallet(Wallet wallet);
    }
}