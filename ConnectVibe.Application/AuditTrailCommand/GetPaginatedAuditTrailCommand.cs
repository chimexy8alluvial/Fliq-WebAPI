
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities;
using MediatR;

namespace Fliq.Application.AuditTrailCommand
{
    public class GetPaginatedAuditTrailCommand : IRequest<ErrorOr<PaginationResponse<AuditTrail>>>
    {
        public PaginationRequest PaginationRequest { get; set; }
    }

    public class GetPaginatedAuditTrailCommandHandler : IRequestHandler<GetPaginatedAuditTrailCommand, ErrorOr<PaginationResponse<AuditTrail>>>
    {
        private readonly IAuditTrailRepository _auditTrailRepository;
        private readonly ILoggerManager _loggerManager;
        public GetPaginatedAuditTrailCommandHandler(IAuditTrailRepository auditTrailRepository, ILoggerManager logger)
        {
            _auditTrailRepository = auditTrailRepository;
            _loggerManager = logger;
        }
        public async Task<ErrorOr<PaginationResponse<AuditTrail>>> Handle(GetPaginatedAuditTrailCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var trails = await _auditTrailRepository.GetAllAuditTrailsAsync(command.PaginationRequest);

                var totalTrails = await _auditTrailRepository.GetTotalAuditTrailCountAsync();
                var totalPages = (int)Math.Ceiling((double)totalTrails / command.PaginationRequest.PageSize);

                return new PaginationResponse<AuditTrail>(trails, totalTrails, command.PaginationRequest.PageNumber, command.PaginationRequest.PageSize);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error fetching paginated audit trails: {ex.Message}");
                return Error.Failure("Failed to fetch audit trails.");
            }
        }
    }
}