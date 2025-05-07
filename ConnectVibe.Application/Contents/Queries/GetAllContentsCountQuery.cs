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
    public record GetAllContentsCountQuery() : IRequest<ErrorOr<CountResult>>;

    public class GetAllContentsCountQueryHandler : IRequestHandler<GetAllContentsCountQuery, ErrorOr<CountResult>>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IContentRepository _contentRepository;

        public GetAllContentsCountQueryHandler(IRepositoryFactory repositoryFactory, IContentRepository contentRepository)
        {
            _repositoryFactory = repositoryFactory;
            _contentRepository = contentRepository;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetAllContentsCountQuery request, CancellationToken cancellationToken)
        {
            var totalCount = await _contentRepository.GetTotalContentCountAsync();
            return new CountResult(totalCount);
        }
    }
}
