
namespace Fliq.Application.AuditTrail.Common
{
    public record GetAuditTrailListResponse
    {
        public List<AuditTrailListItem> List { get; }
        public int TotalCount { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public GetAuditTrailListResponse(List<AuditTrailListItem> list, int totalCount, int pageNumber, int pageSize)
        {
            List = list;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class AuditTrailListItem
    {
        public int UserId { get; set; }
        public string? Name { get; set; }  
        public string? Email { get; set; }       
        public string? AccessType { get; set; }  
        public string? IPAddress { get; set; }
        public string? AuditAction { get; set; }
    }
}

