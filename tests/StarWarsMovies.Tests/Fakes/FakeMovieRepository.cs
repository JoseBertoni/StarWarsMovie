using StarWarsMovies.Application.Interfaces;
using StarWarsMovies.Domain.Entities;

namespace StarWarsMovies.Tests.Fakes;

public sealed class FakeMovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = [];

    public IReadOnlyCollection<Movie> Movies => _movies;

    public Task<IReadOnlyCollection<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<Movie>>(_movies);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var movie = _movies.FirstOrDefault(movie => movie.Id == id);

        return Task.FromResult(movie);
    }

    public Task<Movie?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        var movie = _movies.FirstOrDefault(movie => movie.ExternalId == externalId);

        return Task.FromResult(movie);
    }

    public Task AddAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        _movies.Add(movie);

        return Task.CompletedTask;
    }

    public void Update(Movie movie)
    {
    }

    public void Delete(Movie movie)
    {
        _movies.Remove(movie);
    }
}