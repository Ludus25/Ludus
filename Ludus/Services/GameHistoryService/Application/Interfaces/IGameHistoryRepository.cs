using Common.Entities;

namespace Interfaces
{
    public interface IGameHistoryRepository
    {
        Task SaveGameAsync(GameHistory gameHistory, CancellationToken ct);
        Task<IEnumerable<GameHistory>> GetGamesByUserAsync(string userId, int limit, int offset, CancellationToken ct);
        Task<IEnumerable<GameHistory>> GetGamesByEmailAsync(string userEmail, int limit, int offset, CancellationToken ct);
        Task<GameHistory?> GetByMatchIdAsync(string matchId, CancellationToken ct);
    }
}
