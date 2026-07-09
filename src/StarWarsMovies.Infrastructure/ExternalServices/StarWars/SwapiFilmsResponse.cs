using System.Text.Json.Serialization;

namespace StarWarsMovies.Infrastructure.ExternalServices.StarWars;

public sealed class SwapiFilmsResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public List<SwapiFilmResult> Result { get; set; } = [];
}