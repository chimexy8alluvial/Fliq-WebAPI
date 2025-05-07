using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.Contents.Queries
{
    public record GetAllContentsCountQuery() : IRequest<ErrorOr<CountResult>>;

    public class GetAllContentsCountQueryHandler : IRequestHandler<GetAllContentsCountQuery, ErrorOr<CountResult>>
    {
        private readonly IContentRepository _contentRepository;

        public GetAllContentsCountQueryHandler(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetAllContentsCountQuery request, CancellationToken cancellationToken)
        {
            var totalCount = await _contentRepository.GetTotalContentCountAsync();
            return new CountResult(totalCount);
        }
    }
}
