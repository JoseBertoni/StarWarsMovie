using StarWarsMovies.Domain.Enums;

namespace StarWarsMovies.Domain.Entities;

public sealed class MovieExternalResource
{
    public Guid Id { get; private set; }

    public Guid MovieId { get; private set; }

    public MovieExternalResourceType Type { get; private set; }

    public string Url { get; private set; } = string.Empty;

    public Movie? Movie { get; private set; }

    private MovieExternalResource()
    {
    }

    public MovieExternalResource(Guid movieId, MovieExternalResourceType type, string url)
    {
        if (movieId == Guid.Empty)
            throw new ArgumentException("MovieId is required.", nameof(movieId));

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Resource URL is required.", nameof(url));

        Id = Guid.NewGuid();
        MovieId = movieId;
        Type = type;
        Url = url.Trim();
    }
}