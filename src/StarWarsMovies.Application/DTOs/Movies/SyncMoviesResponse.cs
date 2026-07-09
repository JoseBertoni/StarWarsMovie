namespace StarWarsMovies.Application.DTOs.Movies;

public sealed record SyncMoviesResponse(
    int Created,
    int Updated,
    int TotalProcessed,
    DateTime SyncedAtUtc);