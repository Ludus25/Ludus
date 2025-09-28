using MatchmakingService.Entities;
using MatchmakingService.Application.Services;
using MatchmakingService.Infrastructure.Grpc;
using System.Linq;

namespace MatchmakingService.Application.Commands
{
    public class JoinCommandHandler
    {
        private readonly IMatchRepository _repo;
        private readonly IEventPublisher _publisher;
        private readonly UserGrpcClient? _userClient;

        public JoinCommandHandler(IMatchRepository repo, IEventPublisher publisher, UserGrpcClient? userClient = null)
        {
            _repo = repo;
            _publisher = publisher;
            _userClient = userClient;
        }

        public async Task<string> HandleAsync(JoinCommand cmd)
        {
            if (_repo.IsPlayerInQueue(cmd.PlayerId))
                return "Player already in queue";

            // Get rating from gRPC service if available, otherwise use provided rating
            int rating = cmd.Rating;
            if (_userClient != null)
            {
                var fetchedRating = await _userClient.GetRatingAsync(cmd.PlayerId);
                if (fetchedRating.HasValue)
                {
                    rating = fetchedRating.Value;
                    Console.WriteLine($"[GRPC] Retrieved rating {rating} for player {cmd.PlayerId}");
                }
                else
                {
                    Console.WriteLine($"[GRPC] Failed to retrieve rating for player {cmd.PlayerId}, using provided rating {rating}");
                }
            }

            var player = new PlayerInQueue(cmd.PlayerId, rating);
            _repo.EnqueuePlayer(player);

            // Try form match
            var snapshot = _repo.GetQueueSnapshot();
            if (snapshot.Count >= 2)
            {
                var players = snapshot.Take(2).ToList();
                foreach (var p in players) _repo.RemoveFromQueue(p.PlayerId);

                var match = new Match(players);
                _repo.AddMatch(match);

                // publish event
                _publisher.PublishMatchCreated(new
                {
                    MatchId = match.MatchId,
                    Players = match.Players.Select(p => new { 
                        PlayerId = p.PlayerId, 
                        Rating = p.Rating 
                    }).ToArray(),
                    CreatedAt = DateTime.UtcNow
                });

                Console.WriteLine($"[EVENT] Match created: {match.MatchId} with players: {string.Join(", ", players.Select(p => $"{p.PlayerId}({p.Rating})"))}");
                return $"Match created! MatchId: {match.MatchId}";
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