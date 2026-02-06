using Microsoft.Extensions.Caching.Hybrid;

namespace Infrastructure.Services;

public class CacheService(HybridCache cache) : ICacheService
{
    private readonly HybridCache _cache = cache;

    public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        var options = expiration.HasValue
            ? new HybridCacheEntryOptions
            {
                Expiration = expiration.Value,
                LocalCacheExpiration = expiration.Value
            }
            : null;

        return await _cache.GetOrCreateAsync(
            key,
            async token => await factory(token),
            options,
            tags,
            cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        var options = expiration.HasValue
            ? new HybridCacheEntryOptions
            {
                Expiration = expiration.Value,
                LocalCacheExpiration = expiration.Value
            }
            : null;

        await _cache.SetAsync(key, value, options, tags, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => await _cache.RemoveAsync(key, cancellationToken);

    public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
        => await _cache.RemoveByTagAsync(tag, cancellationToken);
}
