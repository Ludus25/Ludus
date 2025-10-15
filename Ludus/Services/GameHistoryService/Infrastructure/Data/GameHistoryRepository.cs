using Microsoft.EntityFrameworkCore;

using Common.Entities;
using Interfaces;

namespace Data
{
    public class GameHistoryRepository : IGameHistoryRepository
    {
        private readonly GameHistoryDbContext _db;

        public GameHistoryRepository(GameHistoryDbContext db)
        {
            _db = db;
        }

        public async Task SaveGameAsync(GameHistory gameHistory, CancellationToken ct)
        {
            var existing = await _db.GameHistories.FirstOrDefaultAsync(g => g.MatchId == gameHistory.MatchId, ct);
            if (existing != null)
            {
                existing.EndedAt = gameHistory.EndedAt;
                existing.StartedAt = gameHistory.StartedAt;
                existing.MoveHistory = gameHistory.MoveHistory;
                existing.WinnerUserId = gameHistory.WinnerUserId;
                existing.PlayerUserIds = gameHistory.PlayerUserIds;
                _db.GameHistories.Update(existing);
            }
            else
            {
                await _db.GameHistories.AddAsync(gameHistory, ct);
            }
            
            await _db.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<GameHistory>> GetGamesByUserAsync(string userId, int limit, int offset, CancellationToken ct)
        {
            return await _db.GameHistories.AsNoTracking().Where(g => g.PlayerUserIds.Contains(userId)).OrderByDescending(g => g.EndedAt).Skip(offset).Take(limit).ToListAsync(ct);
        }

        public async Task<IEnumerable<GameHistory>> GetGamesByEmailAsync(string userEmail, int limit, int offset, CancellationToken ct)
        {
            return await _db.GameHistories.AsNoTracking().Where(g => g.PlayerEmails.Contains(userEmail)).OrderByDescending(g => g.EndedAt).Skip(offset).Take(limit).ToListAsync(ct);
        }

        public async Task<GameHistory?> GetByMatchIdAsync(string matchId, CancellationToken ct)
        {
            return await _db.GameHistories.FirstOrDefaultAsync(g => g.MatchId == matchId, ct);
        }
    }
}