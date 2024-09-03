using Fliq.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface ISubscriptionRepository
    {
        void Add(Subscription subscription);

        Subscription? GetSubscriptionByUserIdAndProductIdAsync(int userId, string productId);

        void Update(Subscription subscription);
    }
}