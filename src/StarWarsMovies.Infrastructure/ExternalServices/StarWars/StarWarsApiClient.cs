using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using StarWarsMovies.Application.Interfaces;
using StarWarsMovies.Domain.Entities;
using StarWarsMovies.Domain.Enums;

namespace StarWarsMovies.Infrastructure.ExternalServices.StarWars;

public sealed class StarWarsApiClient : IStarWarsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StarWarsApiClient> _logger;

    public StarWarsApiClient(
        HttpClient httpClient,
        ILogger<StarWarsApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Movie>> GetMoviesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<SwapiFilmsResponse>(
            "films",
            cancellationToken);

        if (response?.Result is null || response.Result.Count == 0)
        {
            _logger.LogWarning("Star Wars API returned no films.");
            return [];
        }

        var movies = new List<Movie>();

        foreach (var film in response.Result)
        {
            var properties = film.Properties;

            var movie = new Movie(
                externalId: film.Uid,
                title: properties.Title,
                episodeId: properties.EpisodeId,
                openingCrawl: properties.OpeningCrawl,
                director: properties.Director,
                producer: properties.Producer,
                releaseDate: properties.ReleaseDate,
                sourceUrl: properties.Url,
                isFromExternalApi: true);

            var resources = new List<(MovieExternalResourceType Type, string Url)>();

            resources.AddRange(properties.Characters.Select(url => (MovieExternalResourceType.Character, url)));
            resources.AddRange(properties.Planets.Select(url => (MovieExternalResourceType.Planet, url)));
            resources.AddRange(properties.Species.Select(url => (MovieExternalResourceType.Species, url)));
            resources.AddRange(properties.Starships.Select(url => (MovieExternalResourceType.Starship, url)));
            resources.AddRange(properties.Vehicles.Select(url => (MovieExternalResourceType.Vehicle, url)));

            movie.ReplaceExternalResources(resources);

            movies.Add(movie);
        }

        return movies;
    }
}