using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Infrastructure.Persistence.Repositories;
using MediatR;

namespace Fliq.Application.DatingEnvironment.Queries.SpeedDates
{
    public record GetPaginatedSpeedDatesForAdminQuery(int PageSize, int PageNumber, int? CreationStatus) : IRequest<ErrorOr<PaginationResponse<SpeedDatingEvent>>>;

    public class GetPaginatedSpeedDatesForAdminQueryHandler : IRequestHandler<GetPaginatedSpeedDatesForAdminQuery, ErrorOr<PaginationResponse<SpeedDatingEvent>>>
    {
        private readonly ISpeedDatingEventRepository _speedDatingEventRepository;
        private readonly ILoggerManager _logger;

        public GetPaginatedSpeedDatesForAdminQueryHandler(ISpeedDatingEventRepository speedDatingEventRepository, ILoggerManager logger)
        {
            _speedDatingEventRepository = speedDatingEventRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<PaginationResponse<SpeedDatingEvent>>> Handle(GetPaginatedSpeedDatesForAdminQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch paginated blind dates for Admins
                var speedDates = await _speedDatingEventRepository.GetSpeedDatesForAdmin(query.PageSize, query.PageNumber, query.CreationStatus);
                var speedDateCount = speedDates.Count();

                return new PaginationResponse<SpeedDatingEvent>(speedDates, speedDateCount, query.PageNumber, query.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching paginated speed dates for admins: {ex.Message}");
                return Error.Failure("Failed to fetch paginated speed dates for admins");
            }
        }
    }
}
