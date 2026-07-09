using System.Text.Json.Serialization;

namespace StarWarsMovies.Infrastructure.ExternalServices.StarWars;

public sealed class SwapiFilmProperties
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("episode_id")]
    public int EpisodeId { get; set; }

    [JsonPropertyName("opening_crawl")]
    public string OpeningCrawl { get; set; } = string.Empty;

    [JsonPropertyName("director")]
    public string Director { get; set; } = string.Empty;

    [JsonPropertyName("producer")]
    public string Producer { get; set; } = string.Empty;

    [JsonPropertyName("release_date")]
    public DateOnly ReleaseDate { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("characters")]
    public List<string> Characters { get; set; } = [];

    [JsonPropertyName("planets")]
    public List<string> Planets { get; set; } = [];

    [JsonPropertyName("species")]
    public List<string> Species { get; set; } = [];

    [JsonPropertyName("starships")]
    public List<string> Starships { get; set; } = [];

    [JsonPropertyName("vehicles")]
    public List<string> Vehicles { get; set; } = [];
}