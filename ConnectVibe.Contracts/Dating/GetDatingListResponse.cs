
using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Contracts.Dating
{
    public record GetDatingListResponse
    {
        public List<DatingListItem> List { get; }
        public int TotalCount { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public GetDatingListResponse(List<DatingListItem> list, int totalCount, int page, int pageSize)
        {
            List = list;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }

    public class DatingListItem
    {
        //public int Id { get; set; }
        public string? Title { get; set; }
        public DatingType? Type { get; set; }
        public string? CreatedBy { get; set; }
        public string? SubscriptionType { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}

