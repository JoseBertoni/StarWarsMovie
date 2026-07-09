using StarWarsMovies.Domain.Entities;

namespace StarWarsMovies.Application.Interfaces;

public interface IMovieRepository
{
    Task<IReadOnlyCollection<Movie>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Movie?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);

    Task AddAsync(Movie movie, CancellationToken cancellationToken = default);

    void Update(Movie movie);

    void Delete(Movie movie);
}