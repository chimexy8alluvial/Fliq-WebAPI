namespace Fliq.Application.DashBoard.Common
{
    public record GetUsersResult
    (
       string DisplayName,
        string Email,
        string SubscriptionType,
        DateTime DateJoined,
        DateTime LastOnline
    );
}
