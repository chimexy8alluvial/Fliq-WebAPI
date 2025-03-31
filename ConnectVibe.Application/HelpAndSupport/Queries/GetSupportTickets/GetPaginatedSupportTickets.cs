using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.HelpAndSupport;
using MediatR;

namespace Fliq.Application.HelpAndSupport.Queries.GetSupportTickets
{
    public class GetPaginatedSupportTicketsQuery : IRequest<ErrorOr<PaginationResponse<SupportTicket>>>
    {
        public PaginationRequest PaginationRequest { get; set; }
    }

    public class GetPaginatedSupportTicketsQueryHandler : IRequestHandler<GetPaginatedSupportTicketsQuery, ErrorOr<PaginationResponse<SupportTicket>>>
    {
        private readonly ISupportTicketRepository _repository;
        private readonly ILoggerManager _logger;

        public GetPaginatedSupportTicketsQueryHandler(ISupportTicketRepository repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ErrorOr<PaginationResponse<SupportTicket>>> Handle(
            GetPaginatedSupportTicketsQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                // Fetch paginated support tickets
                var tickets = await _repository.GetPaginatedSupportTicketsAsync(
                    query.PaginationRequest
                );

                // Calculate total pages (assuming you have a method to get the total count of tickets)
                var totalTickets = await _repository.GetTotalSupportTicketsCountAsync();
                var totalPages = (int)Math.Ceiling((double)totalTickets / query.PaginationRequest.PageSize);

                return new PaginationResponse<SupportTicket>(tickets, totalTickets, query.PaginationRequest.PageNumber, query.PaginationRequest.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching paginated support tickets: {ex.Message}");
                return Error.Failure("Failed to fetch support tickets.");
            }
        }
    }
}