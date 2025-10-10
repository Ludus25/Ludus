using MatchmakingService.Entities;
using System.Collections.Generic;

namespace MatchmakingService.Application.Services
{
    public interface IMatchRepository
    {
        void EnqueuePlayer(PlayerInQueue player);
        bool IsPlayerInQueue(string playerId);
        List<PlayerInQueue> GetQueueSnapshot();
        List<Match> GetMatchesSnapshot();
        void AddMatch(Match match);
        Match? GetMatchForPlayer(string playerId);
        bool RemoveFromQueue(string playerId);
    }
}
