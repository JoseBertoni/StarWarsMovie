using StarWarsMovies.Application.Interfaces;
using StarWarsMovies.Domain.Entities;
using StarWarsMovies.Domain.Enums;

namespace StarWarsMovies.Tests.Fakes;

public sealed class FakeStarWarsApiClient : IStarWarsApiClient
{
    public Task<IReadOnlyCollection<Movie>> GetMoviesAsync(CancellationToken cancellationToken = default)
    {
        var movie = new Movie(
            externalId: "1",
            title: "A New Hope",
            episodeId: 4,
            openingCrawl: "It is a period of civil war.",
            director: "George Lucas",
            producer: "Gary Kurtz, Rick McCallum",
            releaseDate: new DateOnly(1977, 5, 25),
            sourceUrl: "https://www.swapi.tech/api/films/1",
            isFromExternalApi: true);

        movie.ReplaceExternalResources([
            (MovieExternalResourceType.Character, "https://www.swapi.tech/api/people/1"),
            (MovieExternalResourceType.Planet, "https://www.swapi.tech/api/planets/1")
        ]);

        return Task.FromResult<IReadOnlyCollection<Movie>>([movie]);
    }
}