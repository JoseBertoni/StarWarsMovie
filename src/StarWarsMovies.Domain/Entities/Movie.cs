using StarWarsMovies.Domain.Enums;

namespace StarWarsMovies.Domain.Entities;

public sealed class Movie
{
    public Guid Id { get; private set; }

    public string? ExternalId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public int EpisodeId { get; private set; }

    public string OpeningCrawl { get; private set; } = string.Empty;

    public string Director { get; private set; } = string.Empty;

    public string Producer { get; private set; } = string.Empty;

    public DateOnly ReleaseDate { get; private set; }

    public string SourceUrl { get; private set; } = string.Empty;

    public bool IsFromExternalApi { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public List<MovieExternalResource> ExternalResources { get; private set; } = [];

    private Movie()
    {
    }

    public Movie(
        string? externalId,
        string title,
        int episodeId,
        string openingCrawl,
        string director,
        string producer,
        DateOnly releaseDate,
        string sourceUrl,
        bool isFromExternalApi)
    {
        Validate(title, episodeId, director, producer);

        Id = Guid.NewGuid();
        ExternalId = string.IsNullOrWhiteSpace(externalId) ? null : externalId.Trim();
        Title = title.Trim();
        EpisodeId = episodeId;
        OpeningCrawl = openingCrawl?.Trim() ?? string.Empty;
        Director = director.Trim();
        Producer = producer.Trim();
        ReleaseDate = releaseDate;
        SourceUrl = sourceUrl?.Trim() ?? string.Empty;
        IsFromExternalApi = isFromExternalApi;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Update(
        string title,
        int episodeId,
        string openingCrawl,
        string director,
        string producer,
        DateOnly releaseDate,
        string? sourceUrl = null)
    {
        Validate(title, episodeId, director, producer);

        Title = title.Trim();
        EpisodeId = episodeId;
        OpeningCrawl = openingCrawl?.Trim() ?? string.Empty;
        Director = director.Trim();
        Producer = producer.Trim();
        ReleaseDate = releaseDate;

        if (!string.IsNullOrWhiteSpace(sourceUrl))
            SourceUrl = sourceUrl.Trim();

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ReplaceExternalResources(IEnumerable<(MovieExternalResourceType Type, string Url)> resources)
    {
        ExternalResources.Clear();

        foreach (var resource in resources)
        {
            if (string.IsNullOrWhiteSpace(resource.Url))
                continue;

            ExternalResources.Add(new MovieExternalResource(Id, resource.Type, resource.Url));
        }

        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static void Validate(string title, int episodeId, string director, string producer)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Movie title is required.", nameof(title));

        if (episodeId <= 0)
            throw new ArgumentException("Episode id must be greater than zero.", nameof(episodeId));

        if (string.IsNullOrWhiteSpace(director))
            throw new ArgumentException("Director is required.", nameof(director));

        if (string.IsNullOrWhiteSpace(producer))
            throw new ArgumentException("Producer is required.", nameof(producer));
    }
}