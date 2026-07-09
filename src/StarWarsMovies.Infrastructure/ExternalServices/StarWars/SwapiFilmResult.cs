using System.Text.Json.Serialization;

namespace StarWarsMovies.Infrastructure.ExternalServices.StarWars;

public sealed class SwapiFilmResult
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("properties")]
    public SwapiFilmProperties Properties { get; set; } = new();
}