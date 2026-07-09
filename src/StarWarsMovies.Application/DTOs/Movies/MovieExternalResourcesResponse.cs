namespace StarWarsMovies.Application.DTOs.Movies;

public sealed record MovieExternalResourcesResponse(
    IReadOnlyCollection<string> Characters,
    IReadOnlyCollection<string> Planets,
    IReadOnlyCollection<string> Species,
    IReadOnlyCollection<string> Starships,
    IReadOnlyCollection<string> Vehicles);