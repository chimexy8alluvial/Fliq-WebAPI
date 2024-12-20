using Microsoft.AspNetCore.SignalR;

namespace Fliq.Application.Common.Hubs
{
    public class GameHub : Hub
    {
        public async Task JoinGameSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
            await Clients.Group(sessionId).SendAsync("UserJoined", $"{Context.ConnectionId} joined session {sessionId}");
        }

        public async Task LeaveGameSession(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
            await Clients.Group(sessionId).SendAsync("UserLeft", $"{Context.ConnectionId} left session {sessionId}");
        }
    }
}