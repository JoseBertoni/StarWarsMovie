namespace StarWarsMovies.Application.DTOs.Movies;

public sealed record CreateMovieRequest(
    string Title,
    int EpisodeId,
    string OpeningCrawl,
    string Director,
    string Producer,
    DateOnly ReleaseDate);