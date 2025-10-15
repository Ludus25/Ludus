using Grpc.Net.Client;
using System.Threading.Tasks;
using Game;
using MatchmakingService.Entities;
using Microsoft.Extensions.Configuration;

namespace MatchmakingService.Infrastructure.Grpc
{
    public class GameGrpcClient
    {
        private readonly GameService.GameServiceClient _client;
        
        public GameGrpcClient(IConfiguration configuration)
        {
            var address = configuration["GAME_GRPC_ADDRESS"] 
                          ?? throw new InvalidOperationException("GAME_GRPC_ADDRESS not configured");
            
            Console.WriteLine($"[GRPC-CLIENT] Connecting to Game Service at: {address}");
            
            
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            
            var channel = GrpcChannel.ForAddress(address);
            _client = new GameService.GameServiceClient(channel);
        }

        public async Task<StartGameResponse?> StartGameAsync(string matchId, List<PlayerInQueue> players)
        {
            try
            {
                Console.WriteLine($"[GRPC-CLIENT] Sending StartGame request - MatchId: {matchId}, Players: {players.Count}");
                
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
                Console.WriteLine($"[GRPC-CLIENT] ✅ Game started successfully: {response.GameServerUrl}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GRPC-CLIENT] ❌ Failed to start game: {ex.Message}");
                Console.WriteLine($"[GRPC-CLIENT] Stack trace: {ex.StackTrace}");
                return null;
            }
        }
    }
}