using StackExchange.Redis;
using ProductAssistant.Services.Interface;
using ProductAssistant.Services.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ProductAssistant.Services.Service
{
    /// <summary>
    /// This class will be use to read from and write to cache
    /// Structure in cache Key-Value pair
    /// </summary>
    public class RedisCacheService : IRedisCacheService
    {
        #region Variable declaration
        private readonly IDatabase _cache;
        private readonly ILogger<RedisCacheService> _logger;
        #endregion

        #region Constructor
        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _cache = redis.GetDatabase();
            _logger = logger;
        }
        #endregion

        #region Read and write to cache
        public async Task<string> ReadfromCacheAsync(List<string> keyword)
        {
            var key = NormalizeKey(keyword);
            string? value = await _cache.StringGetAsync(key);
            if (string.IsNullOrEmpty(value))
            {
                _logger.LogDebug($"Cache miss for key: {key}");
            }
            return value ?? string.Empty;
        }

        public async Task WriteToCacheAsync(List<string> keyword, RedisCacheEntry cacheEntry)
        {
            var key = NormalizeKey(keyword);
            string json = JsonSerializer.Serialize(cacheEntry);
            await _cache.StringSetAsync(key , json, TimeSpan.FromDays(1));
        }
        
        string NormalizeKey(List<string> keywords)
        {
            return "product:" + string.Join(":", keywords
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Select(k => k.Trim().ToLowerInvariant())
                .OrderBy(k => k));
        }
        #endregion
    }
}