using StarWarsMovies.Domain.Entities;

namespace StarWarsMovies.Application.Interfaces;

public interface IStarWarsApiClient
{
    Task<IReadOnlyCollection<Movie>> GetMoviesAsync(CancellationToken cancellationToken = default);
}