using StarWarsMovies.Application.DTOs.Movies;

namespace StarWarsMovies.Application.Interfaces;

public interface IMovieService
{
    Task<IReadOnlyCollection<MovieSummaryResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<MovieDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<MovieDetailResponse> CreateAsync(CreateMovieRequest request, CancellationToken cancellationToken = default);

    Task<MovieDetailResponse> UpdateAsync(Guid id, UpdateMovieRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SyncMoviesResponse> SyncFromExternalApiAsync(CancellationToken cancellationToken = default);
}