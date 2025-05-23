
namespace Fliq.Application.DashBoard.Common.UnifiedSearch
{
    public class UnifiedSearchResult
    {
        public List<UserSearchResult> Users { get; set; } = [];
        public List<EventSearchResult> Events { get; set; } = [];
    }

    // DTOs for each entity type's search results
    public class UserSearchResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        // Other relevant properties
    }

    public class EventSearchResult
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public string? ImageUrl { get; set; }
        // Other relevant properties
    }
}
