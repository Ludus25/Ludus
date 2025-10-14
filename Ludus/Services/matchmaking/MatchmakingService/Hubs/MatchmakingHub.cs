using Microsoft.AspNetCore.SignalR;

namespace MatchmakingService.Hubs
{
    public class MatchmakingHub : Hub
    {
        private static readonly Dictionary<string, string> _playerConnections = new();

        public async Task RegisterPlayer(string playerId)
        {
            _playerConnections[playerId] = Context.ConnectionId;
            Console.WriteLine($"[HUB] Player {playerId} connected: {Context.ConnectionId}");
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var player = _playerConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (player.Key != null)
            {
                _playerConnections.Remove(player.Key);
                Console.WriteLine($"[HUB] Player {player.Key} disconnected");
            }
            return base.OnDisconnectedAsync(exception);
        }

        public static string? GetConnectionId(string playerId)
        {
            _playerConnections.TryGetValue(playerId, out var connectionId);
            return connectionId;
        }
    }
}