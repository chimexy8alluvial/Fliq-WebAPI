
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Entities.Prompts;
using MediatR;

namespace Fliq.Application.Contents.Queries
{
    public record GetAllFlaggedContentsCountQuery() : IRequest<ErrorOr<CountResult>>;

    public class GetAllFlaggedContentsCountQueryHandler : IRequestHandler<GetAllFlaggedContentsCountQuery, ErrorOr<CountResult>>
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public GetAllFlaggedContentsCountQueryHandler(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetAllFlaggedContentsCountQuery request, CancellationToken cancellationToken)
        {
            var contentTypes = new Dictionary<Type, Func<Task<int>>>
        {
            { typeof(SpeedDatingEvent),  () => ( _repositoryFactory.GetRepository<SpeedDatingEvent>()).FlaggedCountAsync() },
            { typeof(BlindDate),  () => ( _repositoryFactory.GetRepository<BlindDate>()).FlaggedCountAsync() },
            { typeof(PromptQuestion),  () => ( _repositoryFactory.GetRepository<PromptQuestion>()).FlaggedCountAsync() },
            { typeof(Game), () => ( _repositoryFactory.GetRepository<Game>()).FlaggedCountAsync() },
            { typeof(Events), () => ( _repositoryFactory.GetRepository<Events>()).FlaggedCountAsync() }
        };

            var totalCount = 0;
            foreach (var countFunc in contentTypes.Values)
            {
                totalCount += await countFunc();
            }

            return new CountResult(totalCount);
        }
    }
}
