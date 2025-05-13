

namespace Fliq.Application.MatchedProfile.Common
{
    public class GetRecentUserMatchResult
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureUrl { get; set; }
        public DateTime DateModified { get; set; }

        public GetRecentUserMatchResult() { }

        public GetRecentUserMatchResult(int userId, string firstName, string lastName, string pictureUrl, DateTime dateModified)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            PictureUrl = pictureUrl;
            DateModified = dateModified;
        }
    }
}