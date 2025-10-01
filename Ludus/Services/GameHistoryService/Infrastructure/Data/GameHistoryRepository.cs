using Entities;
using Interfaces;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class GameHistoryRepository : IGameHistoryRepository
    {
        private readonly GameHistoryDbContext _db;

        public GameHistoryRepository(GameHistoryDbContext db)
        {
            _db = db;
        }

        public async Task SaveGameAsync(GameHistory gameHistory)
        {
            var existing = await _db.GameHistories.FindAsync(gameHistory.MatchId);
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
                await _db.GameHistories.AddAsync(gameHistory);
            }
            
            await _db.SaveChangesAsync();
        }

        public async Task AppendChatMessageAsync(string matchId, IEnumerable<ChatMessage> messages)
        {
            var match = await _db.GameHistories.Include(g => g.ChatMessages).FirstOrDefaultAsync(g =>  g.MatchId == matchId);
            if (match == null)
            {
                match = new GameHistory
                {
                    MatchId = matchId,
                    PlayerUserIds = new List<string>(),
                    StartedAt = DateTime.UtcNow,
                    EndedAt = DateTime.UtcNow,
                    MoveHistory = string.Empty,
                    ChatMessages = new List<ChatMessage>()
                };

                await _db.GameHistories.AddAsync(match);
            }

            foreach (var m in messages)
            {
                m.GameMatchId = matchId;
                match.ChatMessages.Add(m);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<GameHistory>> GetGamesByUserAsync(string userId, int limit = 50, int offset = 0)
        {
            return await _db.GameHistories.Where(g => g.PlayerUserIds.Contains(userId)).OrderByDescending(g => g.EndedAt).Skip(offset).Take(limit).Include(g => g.ChatMessages).ToListAsync();
        }

        public async Task<GameHistory?> GetByMatchIdAsync(string matchId)
        {
            return await _db.GameHistories.Include(g => g.ChatMessages).FirstOrDefaultAsync(g => g.MatchId == matchId);
        }
    }
}