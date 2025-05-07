
using ErrorOr;
using Fliq.Application.AuditTrail.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using MediatR;
using Error = ErrorOr.Error;

namespace Fliq.Application.AuditTrailCommand
{
    public record GetPaginatedAuditTrailCommand(
        int PageNumber,
        int PageSize,
        string? Name
    ) : IRequest<ErrorOr<PaginationResponse<AuditTrailListItem>>>;

    public class GetPaginatedAuditTrailCommandHandler : IRequestHandler<GetPaginatedAuditTrailCommand, ErrorOr<PaginationResponse<AuditTrailListItem>>>
    {
        private readonly IAuditTrailRepository _auditTrailRepository;
        private readonly ILoggerManager _loggerManager;
        public GetPaginatedAuditTrailCommandHandler(IAuditTrailRepository auditTrailRepository, ILoggerManager logger)
        {
            _auditTrailRepository = auditTrailRepository;
            _loggerManager = logger;
        }
        public async Task<ErrorOr<PaginationResponse<AuditTrailListItem>>> Handle(GetPaginatedAuditTrailCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _loggerManager.LogInfo($"Fetching all audit trails list");

                var (trails, totalCount) = await _auditTrailRepository.GetAllAuditTrailsAsync(command.PageNumber, command.PageSize, command.Name);

                return new PaginationResponse<AuditTrailListItem>(trails, totalCount, command.PageNumber, command.PageSize);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error fetching paginated audit trails: {ex.Message}");
                return Error.Failure("Failed to fetch audit trails.");
            }
        }
    }
}

