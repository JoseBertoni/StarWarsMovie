namespace StarWarsMovies.Application.DTOs.Movies;

public sealed record MovieDetailResponse(
    Guid Id,
    string? ExternalId,
    string Title,
    int EpisodeId,
    string OpeningCrawl,
    string Director,
    string Producer,
    DateOnly ReleaseDate,
    string SourceUrl,
    bool IsFromExternalApi,
    MovieExternalResourcesResponse ExternalResources);