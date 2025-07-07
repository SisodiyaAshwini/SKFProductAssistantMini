
using ProductAssistant.Services.Models;

namespace ProductAssistant.Services.Interface
{
    public interface IRedisCacheService
    {
        Task<string> ReadfromCacheAsync(List<string> keyword);
        Task WriteToCacheAsync(List<string> keyword, RedisCacheEntry cacheEntry);
    }
}
    