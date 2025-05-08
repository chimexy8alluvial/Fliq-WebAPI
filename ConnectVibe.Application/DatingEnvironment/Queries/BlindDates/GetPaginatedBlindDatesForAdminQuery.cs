using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using MediatR;

namespace Fliq.Application.DatingEnvironment.Queries.BlindDates
{
    public record GetPaginatedBlindDatesForAdminQuery(int PageSize, int PageNumber, int? CreationStatus) : IRequest<ErrorOr<PaginationResponse<BlindDate>>>;

    public class GetPaginatedBlindDatesForAdminQueryHandler : IRequestHandler<GetPaginatedBlindDatesForAdminQuery, ErrorOr<PaginationResponse<BlindDate>>>
    {
        private readonly IBlindDateRepository _blindDateRepository;
        private readonly ILoggerManager _logger;

        public GetPaginatedBlindDatesForAdminQueryHandler(IBlindDateRepository blindDateRepository, ILoggerManager logger)
        {
            _blindDateRepository = blindDateRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<PaginationResponse<BlindDate>>> Handle(GetPaginatedBlindDatesForAdminQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch paginated blind dates for Admins
                var blindDates = await _blindDateRepository.GetBlindDatesForAdmin(query.PageSize, query.PageNumber, query.CreationStatus);
                var blindDateCount = blindDates.Count();

                return new PaginationResponse<BlindDate>(blindDates, blindDateCount, query.PageNumber, query.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching paginated blind dates for admins: {ex.Message}");
                return Error.Failure("Failed to fetch paginated blind dates for admins");
            }
        }
    }
}
