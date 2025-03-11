using Fliq.Domain.Entities;


namespace Fliq.Application.DashBoard.Common
{
    public record CreateUserResult
    (
       string DisplayName,
        string Email,
        string SubscriptionType,
        DateTime DateJoined,
        DateTime LastOnline
    );
}
