using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Entities.Interfaces;
using Fliq.Domain.Entities.Prompts;
using Fliq.Infrastructure.Persistence.Repositories.Adapters;
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

            if (typeof(T) == typeof(BlindDate))
                return new BlindDateRepositoryAdapter(
                    _serviceProvider.GetRequiredService<IBlindDateRepository>()) as IGenericRepository<T>;

            if (typeof(T) == typeof(Game))
                return new GamesRepositoryAdapter(
                    _serviceProvider.GetRequiredService<IGamesRepository>()) as IGenericRepository<T>;

            if (typeof(T) == typeof(PromptQuestion))
                return new PromptRepositoryAdapter(
                    _serviceProvider.GetRequiredService<IPromptQuestionRepository>()) as IGenericRepository<T>;

            if (typeof(T) == typeof(Events))
                return new EventsRepositoryAdapter(
                    _serviceProvider.GetRequiredService<IEventRepository>()) as IGenericRepository<T>;

            return null;
        }
    }
}
