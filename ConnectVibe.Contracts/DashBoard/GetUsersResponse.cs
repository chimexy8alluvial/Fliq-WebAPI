using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Contracts.DashBoard
{
    public record GetUsersResponse
        (string DisplayName,
        string Email,
        string SubscriptionType,
        DateTime DateJoined,
        DateTime LastOnline);
}
