
using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Application.DatingEnvironment.Common
{
    public class GetDatingListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Title { get; set; }
        public DatingType? DatingType { get; set; }
        public string? CreatedBy { get; set; }
        public string? SubscriptionType { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }  
    }
}
