using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using XOGameService.API.Models;

namespace XOGameService.API.Repositories
{
    public class RedisXOGameRepository : IXOGameRepository
    {
        private readonly IDistributedCache _distributedCache;

        public RedisXOGameRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
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
    }
}
