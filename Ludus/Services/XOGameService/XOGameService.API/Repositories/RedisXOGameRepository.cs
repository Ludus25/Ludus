using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using XOGameService.API.Models;
using XOGameService.API.Models.Enums;

namespace XOGameService.API.Repositories
{
    public class RedisXOGameRepository : IXOGameRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _redis;

        public RedisXOGameRepository(IDistributedCache distributedCache, IConnectionMultiplexer redis)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        }

        private static string Key(string gameId) => $"game:{gameId}";

        public async Task<GameState?> GetAsync(string gameId)
        {
            var json = await _distributedCache.GetStringAsync(Key(gameId));
            return string.IsNullOrEmpty(json)
                ? null
                : JsonConvert.DeserializeObject<GameState>(json);
        }

        public async Task CreateAsync(GameState gameState)
        {
            var json = JsonConvert.SerializeObject(gameState);
            await _distributedCache.SetStringAsync(
                Key(gameState.GameId),
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                });
        }

        public async Task<bool> TryUpdateAsync(GameState newState, int expectedVersion)
        {
            var currentGame = await GetAsync(newState.GameId);

            if (currentGame == null) return false;
            if (currentGame.Version != expectedVersion) return false;

            var json = JsonConvert.SerializeObject(newState);
            await _distributedCache.SetStringAsync(
                Key(newState.GameId),
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                });
            return true;
        }

        public async Task<GameState?> GetGameByUserIdAndStatus(string userId, GameStatus gameStatus)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: "game:*");

            GameState? mostRecent = null;

            foreach (var key in keys)
            {
                var json = await _distributedCache.GetStringAsync(key);
                if (string.IsNullOrEmpty(json)) continue;

                var game = JsonConvert.DeserializeObject<GameState>(json);
                if (game == null) continue;

                bool isParticipant = game.PlayerXId == userId || game.PlayerOId == userId;
                if (isParticipant && game.Status == gameStatus)
                {
                    if (mostRecent == null || game.UpdatedAt > mostRecent.UpdatedAt)
                        mostRecent = game;
                }
            }

            return mostRecent;
        }
    }
}
