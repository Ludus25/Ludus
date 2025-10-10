using MatchmakingService.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MatchmakingService.Application.Services
{
    public class InMemoryMatchRepository : IMatchRepository
    {
        private readonly List<PlayerInQueue> _queue = new();
        private readonly List<Match> _matches = new();
        private readonly object _lock = new();

        public void EnqueuePlayer(PlayerInQueue player)
        {
            lock (_lock)
            {
                _queue.Add(player);
            }
        }

        public bool IsPlayerInQueue(string playerId)
        {
            lock (_lock) { return _queue.Any(p => p.PlayerId == playerId); }
        }

        public List<PlayerInQueue> GetQueueSnapshot()
        {
            lock (_lock) { return _queue.ToList(); }
        }

        public List<Match> GetMatchesSnapshot()
        {
            lock (_lock) { return _matches.ToList(); }
        }

        public void AddMatch(Match match)
        {
            lock (_lock) { _matches.Add(match); }
        }

        public Match? GetMatchForPlayer(string playerId)
        {
            lock (_lock) { return _matches.FirstOrDefault(m => m.Players.Any(p => p.PlayerId == playerId)); }
        }

        public bool RemoveFromQueue(string playerId)
        {
            lock (_lock)
            {
                var p = _queue.FirstOrDefault(x => x.PlayerId == playerId);
                if (p == null) return false;
                _queue.Remove(p);
                return true;
            }
        }
    }
}
