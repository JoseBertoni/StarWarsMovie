using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StarWarsMovies.Application.Interfaces;

namespace StarWarsMovies.Infrastructure.Cache;

public sealed class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisCacheService> logger)
    {
        _database = connectionMultiplexer.GetDatabase();
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Redis GET failed for key {CacheKey}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, JsonOptions);

            await _database.StringSetAsync(key, json, expiration);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Redis SET failed for key {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Redis REMOVE failed for key {CacheKey}", key);
        }
    }
}