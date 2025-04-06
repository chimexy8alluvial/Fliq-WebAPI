
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IGenericRepository<T>? GetRepository<T>() where T : class, IApprovableContent
        {
            if (typeof(T) == typeof(SpeedDatingEvent))
                return new SpeedDatingEventRepositoryAdapter(
                    _serviceProvider.GetRequiredService<ISpeedDatingEventRepository>()) as IGenericRepository<T>;

            //if (typeof(T) == typeof(BlindDate))
            //    return new BlindDateRepositoryAdapter(
            //        _serviceProvider.GetRequiredService<IBlindDateRepository>()) as IGenericRepository<T>;

            return null;
        }
    }
}
