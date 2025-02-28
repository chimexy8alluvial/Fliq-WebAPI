

using Microsoft.AspNetCore.SignalR;

namespace Fliq.Application.Common.Hubs
{
    public class BlindDateHub : Hub
    {
        public async Task JoinSession(int blindDateId, int userId)
        {
            string groupName = $"BlindDate-{blindDateId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("UserJoined", userId);
        }

        public async Task SendOffer(int blindDateId, string offer, int userId)
        {
            string groupName = $"BlindDate-{blindDateId}";
            await Clients.Group(groupName).SendAsync("ReceiveOffer", offer, userId);
        }

        public async Task SendAnswer(int blindDateId, string answer, int userId)
        {
            string groupName = $"BlindDate-{blindDateId}";
            await Clients.Group(groupName).SendAsync("ReceiveAnswer", answer, userId);
        }

        public async Task SendIceCandidate(int blindDateId, string iceCandidate, int userId)
        {
            string groupName = $"BlindDate-{blindDateId}";
            await Clients.Group(groupName).SendAsync("ReceiveIceCandidate", iceCandidate, userId);
        }
    }
}
