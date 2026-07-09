using StarWarsMovies.Application.Interfaces;

namespace StarWarsMovies.Tests.Fakes;

public sealed class FakeCacheService : ICacheService
{
    private readonly Dictionary<string, object> _cache = [];

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var value) && value is T typedValue)
            return Task.FromResult<T?>(typedValue);

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        if (value is not null)
            _cache[key] = value;

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);

        return Task.CompletedTask;
    }
}