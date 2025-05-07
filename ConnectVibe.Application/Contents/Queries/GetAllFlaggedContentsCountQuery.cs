
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.Contents.Queries
{
    public record GetAllFlaggedContentsCountQuery() : IRequest<ErrorOr<CountResult>>;

    public class GetAllFlaggedContentsCountQueryHandler : IRequestHandler<GetAllFlaggedContentsCountQuery, ErrorOr<CountResult>>
    {
        private readonly IContentRepository _contentRepository;

        public GetAllFlaggedContentsCountQueryHandler(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetAllFlaggedContentsCountQuery request, CancellationToken cancellationToken)
        {
            var totalFlaggedCount = await _contentRepository.GetTotalFlaggedContentCountAsync();
            return new CountResult(totalFlaggedCount);
        }
    }
}
