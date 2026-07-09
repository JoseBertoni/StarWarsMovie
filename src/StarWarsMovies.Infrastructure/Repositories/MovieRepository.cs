using Microsoft.EntityFrameworkCore;
using StarWarsMovies.Application.Interfaces;
using StarWarsMovies.Domain.Entities;
using StarWarsMovies.Infrastructure.Persistence;

namespace StarWarsMovies.Infrastructure.Repositories;

public sealed class MovieRepository : IMovieRepository
{
    private readonly AppDbContext _dbContext;

    public MovieRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Movies
            .AsNoTracking()
            .Include(movie => movie.ExternalResources)
            .OrderBy(movie => movie.EpisodeId)
            .ToListAsync(cancellationToken);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Movies
            .Include(movie => movie.ExternalResources)
            .FirstOrDefaultAsync(movie => movie.Id == id, cancellationToken);
    }

    public Task<Movie?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Movies
            .Include(movie => movie.ExternalResources)
            .FirstOrDefaultAsync(movie => movie.ExternalId == externalId, cancellationToken);
    }

    public async Task AddAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _dbContext.Movies.AddAsync(movie, cancellationToken);
    }

    public void Update(Movie movie)
    {
        _dbContext.Movies.Update(movie);
    }

    public void Delete(Movie movie)
    {
        _dbContext.Movies.Remove(movie);
    }
}