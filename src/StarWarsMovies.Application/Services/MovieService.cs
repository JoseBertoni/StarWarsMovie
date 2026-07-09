using StarWarsMovies.Application.Common.Exceptions;
using StarWarsMovies.Application.DTOs.Movies;
using StarWarsMovies.Application.Interfaces;
using StarWarsMovies.Domain.Entities;
using StarWarsMovies.Domain.Enums;

namespace StarWarsMovies.Application.Services;

public sealed class MovieService : IMovieService
{
    private const string MoviesListCacheKey = "movies:all";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

    private readonly IMovieRepository _movieRepository;
    private readonly IStarWarsApiClient _starWarsApiClient;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public MovieService(
        IMovieRepository movieRepository,
        IStarWarsApiClient starWarsApiClient,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _movieRepository = movieRepository;
        _starWarsApiClient = starWarsApiClient;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<MovieSummaryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cachedMovies = await _cacheService.GetAsync<IReadOnlyCollection<MovieSummaryResponse>>(
            MoviesListCacheKey,
            cancellationToken);

        if (cachedMovies is not null)
            return cachedMovies;

        var movies = await _movieRepository.GetAllAsync(cancellationToken);

        var response = movies
            .OrderBy(movie => movie.EpisodeId)
            .Select(MapToSummaryResponse)
            .ToList();

        await _cacheService.SetAsync(
            MoviesListCacheKey,
            response,
            CacheExpiration,
            cancellationToken);

        return response;
    }

    public async Task<MovieDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetMovieDetailCacheKey(id);

        var cachedMovie = await _cacheService.GetAsync<MovieDetailResponse>(
            cacheKey,
            cancellationToken);

        if (cachedMovie is not null)
            return cachedMovie;

        var movie = await _movieRepository.GetByIdAsync(id, cancellationToken);

        if (movie is null)
            throw new NotFoundException("Movie", id);

        var response = MapToDetailResponse(movie);

        await _cacheService.SetAsync(
            cacheKey,
            response,
            CacheExpiration,
            cancellationToken);

        return response;
    }

    public async Task<MovieDetailResponse> CreateAsync(CreateMovieRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCreateOrUpdateRequest(
            request.Title,
            request.EpisodeId,
            request.Director,
            request.Producer);

        var movie = new Movie(
            externalId: null,
            title: request.Title,
            episodeId: request.EpisodeId,
            openingCrawl: request.OpeningCrawl,
            director: request.Director,
            producer: request.Producer,
            releaseDate: request.ReleaseDate,
            sourceUrl: string.Empty,
            isFromExternalApi: false);

        await _movieRepository.AddAsync(movie, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(MoviesListCacheKey, cancellationToken);

        return MapToDetailResponse(movie);
    }

    public async Task<MovieDetailResponse> UpdateAsync(Guid id, UpdateMovieRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCreateOrUpdateRequest(
            request.Title,
            request.EpisodeId,
            request.Director,
            request.Producer);

        var movie = await _movieRepository.GetByIdAsync(id, cancellationToken);

        if (movie is null)
            throw new NotFoundException("Movie", id);

        movie.Update(
            request.Title,
            request.EpisodeId,
            request.OpeningCrawl,
            request.Director,
            request.Producer,
            request.ReleaseDate);

        _movieRepository.Update(movie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await InvalidateMovieCacheAsync(movie.Id, cancellationToken);

        return MapToDetailResponse(movie);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var movie = await _movieRepository.GetByIdAsync(id, cancellationToken);

        if (movie is null)
            throw new NotFoundException("Movie", id);

        _movieRepository.Delete(movie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await InvalidateMovieCacheAsync(id, cancellationToken);
    }

    public async Task<SyncMoviesResponse> SyncFromExternalApiAsync(CancellationToken cancellationToken = default)
    {
        var externalMovies = await _starWarsApiClient.GetMoviesAsync(cancellationToken);

        var created = 0;
        var updated = 0;

        foreach (var externalMovie in externalMovies)
        {
            if (string.IsNullOrWhiteSpace(externalMovie.ExternalId))
                continue;

            var existingMovie = await _movieRepository.GetByExternalIdAsync(
                externalMovie.ExternalId,
                cancellationToken);

            if (existingMovie is null)
            {
                await _movieRepository.AddAsync(externalMovie, cancellationToken);
                created++;
                continue;
            }

            existingMovie.Update(
                externalMovie.Title,
                externalMovie.EpisodeId,
                externalMovie.OpeningCrawl,
                externalMovie.Director,
                externalMovie.Producer,
                externalMovie.ReleaseDate,
                externalMovie.SourceUrl);

            existingMovie.ReplaceExternalResources(
                externalMovie.ExternalResources.Select(resource => (resource.Type, resource.Url)));

            _movieRepository.Update(existingMovie);
            updated++;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(MoviesListCacheKey, cancellationToken);

        return new SyncMoviesResponse(
            created,
            updated,
            externalMovies.Count,
            DateTime.UtcNow);
    }

    private async Task InvalidateMovieCacheAsync(Guid movieId, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveAsync(MoviesListCacheKey, cancellationToken);
        await _cacheService.RemoveAsync(GetMovieDetailCacheKey(movieId), cancellationToken);
    }

    private static string GetMovieDetailCacheKey(Guid movieId)
    {
        return $"movies:{movieId}";
    }

    private static MovieSummaryResponse MapToSummaryResponse(Movie movie)
    {
        return new MovieSummaryResponse(
            movie.Id,
            movie.ExternalId,
            movie.Title,
            movie.EpisodeId,
            movie.Director,
            movie.Producer,
            movie.ReleaseDate,
            movie.IsFromExternalApi);
    }

    private static MovieDetailResponse MapToDetailResponse(Movie movie)
    {
        return new MovieDetailResponse(
            movie.Id,
            movie.ExternalId,
            movie.Title,
            movie.EpisodeId,
            movie.OpeningCrawl,
            movie.Director,
            movie.Producer,
            movie.ReleaseDate,
            movie.SourceUrl,
            movie.IsFromExternalApi,
            MapExternalResources(movie.ExternalResources));
    }

    private static MovieExternalResourcesResponse MapExternalResources(IEnumerable<MovieExternalResource> resources)
    {
        var resourcesList = resources.ToList();

        return new MovieExternalResourcesResponse(
            resourcesList
                .Where(resource => resource.Type == MovieExternalResourceType.Character)
                .Select(resource => resource.Url)
                .ToList(),
            resourcesList
                .Where(resource => resource.Type == MovieExternalResourceType.Planet)
                .Select(resource => resource.Url)
                .ToList(),
            resourcesList
                .Where(resource => resource.Type == MovieExternalResourceType.Species)
                .Select(resource => resource.Url)
                .ToList(),
            resourcesList
                .Where(resource => resource.Type == MovieExternalResourceType.Starship)
                .Select(resource => resource.Url)
                .ToList(),
            resourcesList
                .Where(resource => resource.Type == MovieExternalResourceType.Vehicle)
                .Select(resource => resource.Url)
                .ToList());
    }

    private static void ValidateCreateOrUpdateRequest(
        string title,
        int episodeId,
        string director,
        string producer)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(title))
            errors["title"] = ["Title is required."];

        if (episodeId <= 0)
            errors["episodeId"] = ["Episode id must be greater than zero."];

        if (string.IsNullOrWhiteSpace(director))
            errors["director"] = ["Director is required."];

        if (string.IsNullOrWhiteSpace(producer))
            errors["producer"] = ["Producer is required."];

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}