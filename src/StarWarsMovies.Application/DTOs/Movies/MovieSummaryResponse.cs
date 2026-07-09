namespace StarWarsMovies.Application.DTOs.Movies;

public sealed record MovieSummaryResponse(
    Guid Id,
    string? ExternalId,
    string Title,
    int EpisodeId,
    string Director,
    string Producer,
    DateOnly ReleaseDate,
    bool IsFromExternalApi);