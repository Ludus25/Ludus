using MatchmakingService.Entities;
using MatchmakingService.Application.Services;
using MatchmakingService.Infrastructure.Grpc;
using MatchmakingService.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace MatchmakingService.Application.Commands
{
    public class JoinCommandHandler
    {
        private readonly IMatchRepository _repo;
        private readonly IEventPublisher _publisher;
        private readonly GameGrpcClient? _gameClient;
        private readonly IHubContext<MatchmakingHub>? _hubContext;

        public JoinCommandHandler(
            IMatchRepository repo, 
            IEventPublisher publisher, 
            GameGrpcClient? gameClient = null,
            IHubContext<MatchmakingHub>? hubContext = null)
        {
            _repo = repo;
            _publisher = publisher;
            _gameClient = gameClient;
            _hubContext = hubContext;
        }

        public async Task<string> HandleAsync(JoinCommand cmd)
        {
            if (_repo.IsPlayerInQueue(cmd.PlayerId))
                return "Player already in queue";

            // Koristi rating direktno iz request-a
            int rating = cmd.Rating;
            Console.WriteLine($"[MATCHMAKING] Player {cmd.PlayerId} joining with rating {rating}");

            var player = new PlayerInQueue(cmd.PlayerId, rating);
            _repo.EnqueuePlayer(player);

            // Try form match
            var snapshot = _repo.GetQueueSnapshot();
            if (snapshot.Count >= 2)
            {
                var players = snapshot.Take(2).ToList();
                foreach (var p in players) 
                    _repo.RemoveFromQueue(p.PlayerId);

                var match = new Match(players);
                _repo.AddMatch(match);

                Console.WriteLine($"[MATCHMAKING] Match found! MatchId: {match.MatchId}");
                Console.WriteLine($"[MATCHMAKING] Players: {string.Join(", ", players.Select(p => $"{p.PlayerId}({p.Rating})"))}");

                // Try gRPC first
                string? gameUrl = null;
                if (_gameClient != null)
                {
                    Console.WriteLine($"[MATCHMAKING] Calling gRPC StartGame...");
                    var response = await _gameClient.StartGameAsync(match.MatchId, players.ToList());
                    gameUrl = response?.GameServerUrl;
                    
                    if (gameUrl != null)
                    {
                        Console.WriteLine($"[MATCHMAKING] gRPC Success! GameServerUrl: {gameUrl}");
                    }
                    else
                    {
                        Console.WriteLine($"[MATCHMAKING] gRPC returned null, falling back to RabbitMQ");
                    }
                }

                // Notifikuj OBA igrača preko SignalR
                if (_hubContext != null)
                {
                    foreach (var p in players)
                    {
                        var connectionId = MatchmakingHub.GetConnectionId(p.PlayerId);
                        if (connectionId != null)
                        {
                            await _hubContext.Clients.Client(connectionId).SendAsync("MatchFound", new
                            {
                                MatchId = match.MatchId,
                                GameUrl = gameUrl,
                                Players = players.Select(pl => pl.PlayerId).ToArray()
                            });
                            Console.WriteLine($"[SIGNALR] Notified player {p.PlayerId}");
                        }
                    }
                }

                // Publish event to RabbitMQ (fallback or parallel notification)
                _publisher.PublishMatchCreated(new
                {
                    MatchId = match.MatchId,
                    Players = match.Players.Select(p => new 
                    { 
                        PlayerId = p.PlayerId, 
                        Rating = p.Rating 
                    }).ToArray(),
                    CreatedAt = DateTime.UtcNow
                });

                Console.WriteLine($"[EVENT] Match created: {match.MatchId} with players: {string.Join(", ", players.Select(p => $"{p.PlayerId}({p.Rating})"))}");

                return gameUrl != null 
                    ? $"Match created! Join at: {gameUrl}" 
                    : $"Match created! MatchId: {match.MatchId}";
            }

            return "Player added to queue";
        }

        // Keep sync version for backward compatibility
        public string Handle(JoinCommand cmd)
        {
            return HandleAsync(cmd).GetAwaiter().GetResult();
        }
    }
}