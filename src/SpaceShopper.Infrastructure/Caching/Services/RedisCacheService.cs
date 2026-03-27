using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql.Internal;
using SpaceShopper.Application.Interfaces.Caching;
using SpaceShopper.Infrastructure.Caching.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceShopper.Infrastructure.Caching.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly CacheSetting _cacheSetting;
        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public RedisCacheService(
            IDistributedCache cache,
            ILogger<RedisCacheService> logger,
            IOptions<CacheSetting> cacheOptions)
        {
            _cache = cache;
            _logger = logger;
            _cacheSetting = cacheOptions.Value;
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            var value = await _cache.GetAsync(key, cancellationToken);
            return value is not null && value.Length > 0;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var bytes = await _cache.GetAsync(key, cancellationToken);

            if (bytes is null || bytes.Length == 0)
            {
                return default;
            }

            try
            {
                var json = System.Text.Encoding.UTF8.GetString(bytes);
                return System.Text.Json.JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing cache entry for key {Key}", key);
                return default;
            }
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return _cache.RemoveAsync(key, cancellationToken);
        }

        public async Task<(bool Success, T? Value)> TryGetValueAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var val = await _cache.GetAsync(key, cancellationToken);
            if (val == null) return (false, default);

            try
            {
                var value = JsonSerializer.Deserialize<T>(val, serializerOptions);
                return (true, value);
            }
            catch
            {
                return (false, default);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? AbsoluteExpiration = null, TimeSpan? SlidingExpiration = null, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = AbsoluteExpiration ?? _cacheSetting.AbsoluteExpiration,
                SlidingExpiration = SlidingExpiration ?? _cacheSetting.SlidingExpiration,
            };

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            await _cache.SetAsync(key, bytes, options, cancellationToken);
        }

        public async Task<T?> GetAndRemoveAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var bytes = await _cache.GetAsync(key, cancellationToken);

            if (bytes is null || bytes.Length == 0)
            {
                return default;
            }

            T? result;
            try
            {
                var json = System.Text.Encoding.UTF8.GetString(bytes);
                result = System.Text.Json.JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing cache entry for key {Key} in GetAndRemoveAsync", key);
                result = default;
            }

            try
            {
                await _cache.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache entry for key {Key} in GetAndRemoveAsync", key);
            }

            return result;
        }
    }
}
