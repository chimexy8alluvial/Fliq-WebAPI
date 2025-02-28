

using Microsoft.AspNetCore.SignalR;

namespace Fliq.Application.Common.Hubs
{
    public class BlindDateHub : Hub
    {
        // Keeps track of connected users in each Blind Date session
        private static readonly Dictionary<int, HashSet<int>> SessionParticipants = new();

        public async Task JoinSession(int blindDateId, int userId)
        {
            string groupName = $"BlindDate-{blindDateId}";

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            lock (SessionParticipants)
            {
                if (!SessionParticipants.ContainsKey(blindDateId))
                {
                    SessionParticipants[blindDateId] = new HashSet<int>();
                }
                SessionParticipants[blindDateId].Add(userId);
            }

            // Notify existing participants about the new user
            await Clients.Group(groupName).SendAsync("UserJoined", userId, SessionParticipants[blindDateId]);
        }

        public async Task SendOffer(int blindDateId, string offer, int senderUserId, int receiverUserId)
        {
            await Clients.User(receiverUserId.ToString()).SendAsync("ReceiveOffer", offer, senderUserId);
        }

        public async Task SendAnswer(int blindDateId, string answer, int senderUserId, int receiverUserId)
        {
            await Clients.User(receiverUserId.ToString()).SendAsync("ReceiveAnswer", answer, senderUserId);
        }

        public async Task SendIceCandidate(int blindDateId, string iceCandidate, int senderUserId, int receiverUserId)
        {
            await Clients.User(receiverUserId.ToString()).SendAsync("ReceiveIceCandidate", iceCandidate, senderUserId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userIdString = Context.GetHttpContext()?.Request.Query["userId"];
            string blindDateIdString = Context.GetHttpContext()?.Request.Query["blindDateId"];

            if (int.TryParse(userIdString, out int userId) && int.TryParse(blindDateIdString, out int blindDateId))
            {
                string groupName = $"BlindDate-{blindDateId}";
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

                lock (SessionParticipants)
                {
                    if (SessionParticipants.ContainsKey(blindDateId))
                    {
                        SessionParticipants[blindDateId].Remove(userId);
                        if (SessionParticipants[blindDateId].Count == 0)
                        {
                            SessionParticipants.Remove(blindDateId);
                        }
                    }
                }

                await Clients.Group(groupName).SendAsync("UserLeft", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}
