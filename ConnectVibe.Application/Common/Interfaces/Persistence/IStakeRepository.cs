using Fliq.Domain.Entities.Games;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IStakeRepository
    {
        Stake Add(Stake stake);

        Stake? GetStakeById(int id);

        Stake? GetStakeByGameSessionId(int id);

        void UpdateStake(Stake stake);
    }
}