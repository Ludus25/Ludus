using Grpc.Net.Client;
using System.Threading.Tasks;
using Game; // generated namespace from proto
using MatchmakingService.Entities;

namespace MatchmakingService.Infrastructure.Grpc
{
    public class GameGrpcClient
    {
        private readonly GameService.GameServiceClient _client;
        
        public GameGrpcClient(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new GameService.GameServiceClient(channel);
        }

        public async Task<StartGameResponse?> StartGameAsync(string matchId, List<PlayerInQueue> players)
        {
            try
            {
                var request = new StartGameRequest
                {
                    MatchId = matchId
                };
                
                foreach (var player in players)
                {
                    request.Players.Add(new Game.Player
                    {
                        PlayerId = player.PlayerId,
                        Rating = player.Rating
                    });
                }
                
                var response = await _client.StartGameAsync(request);
                Console.WriteLine($"[GRPC-GAME] Game started successfully: {response.GameServerUrl}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GRPC-GAME-ERROR] Failed to start game: {ex.Message}");
                return null;
            }
        }
    }
}