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

        public async Task BroadcastDrawing(string sessionId, object drawingAction)
        {
            await Clients.Group(sessionId).SendAsync("ReceiveDrawing", drawingAction);
        }

        public async Task BroadcastGuess(string sessionId, string guess, string playerName)
        {
            await Clients.Group(sessionId).SendAsync("ReceiveGuess", new
            {
                PlayerName = playerName,
                Guess = guess
            });
        }

        public async Task NotifyCorrectGuess(string sessionId, string correctGuess, string playerName)
        {
            await Clients.Group(sessionId).SendAsync("CorrectGuess", new
            {
                PlayerName = playerName,
                CorrectGuess = correctGuess
            });
        }
    }
}