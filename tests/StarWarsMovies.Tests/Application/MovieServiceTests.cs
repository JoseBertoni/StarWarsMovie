using StarWarsMovies.Application.Common.Exceptions;
using StarWarsMovies.Application.DTOs.Movies;
using StarWarsMovies.Application.Services;
using StarWarsMovies.Domain.Entities;
using StarWarsMovies.Tests.Fakes;

namespace StarWarsMovies.Tests.Application;

public sealed class MovieServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateMovie_WhenRequestIsValid()
    {
        var movieRepository = new FakeMovieRepository();
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            movieRepository,
            unitOfWork: unitOfWork);

        var request = new CreateMovieRequest(
            "Rogue One",
            1,
            "Opening crawl",
            "Gareth Edwards",
            "Lucasfilm",
            new DateOnly(2016, 12, 16));

        var response = await service.CreateAsync(request);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Rogue One", response.Title);
        Assert.Equal(1, response.EpisodeId);
        Assert.False(response.IsFromExternalApi);
        Assert.Single(movieRepository.Movies);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenTitleIsEmpty()
    {
        var service = CreateService();

        var request = new CreateMovieRequest(
            "",
            1,
            "Opening crawl",
            "Director",
            "Producer",
            new DateOnly(2024, 1, 1));

        await Assert.ThrowsAsync<ValidationException>(() =>
            service.CreateAsync(request));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMoviesOrderedByEpisodeId()
    {
        var movieRepository = new FakeMovieRepository();

        await movieRepository.AddAsync(CreateMovie("Episode Six", 6));
        await movieRepository.AddAsync(CreateMovie("Episode Four", 4));

        var service = CreateService(movieRepository);

        var result = await service.GetAllAsync();

        var movies = result.ToList();

        Assert.Equal(2, movies.Count);
        Assert.Equal(4, movies[0].EpisodeId);
        Assert.Equal(6, movies[1].EpisodeId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMovie_WhenMovieExists()
    {
        var movieRepository = new FakeMovieRepository();
        var movie = CreateMovie("A New Hope", 4);

        await movieRepository.AddAsync(movie);

        var service = CreateService(movieRepository);

        var response = await service.GetByIdAsync(movie.Id);

        Assert.Equal(movie.Id, response.Id);
        Assert.Equal("A New Hope", response.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenMovieDoesNotExist()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMovie_WhenMovieExists()
    {
        var movieRepository = new FakeMovieRepository();
        var unitOfWork = new FakeUnitOfWork();

        var movie = CreateMovie("Old Title", 4);

        await movieRepository.AddAsync(movie);

        var service = CreateService(
            movieRepository,
            unitOfWork: unitOfWork);

        var request = new UpdateMovieRequest(
            "Updated Title",
            5,
            "Updated opening crawl",
            "Updated Director",
            "Updated Producer",
            new DateOnly(1980, 5, 21));

        var response = await service.UpdateAsync(movie.Id, request);

        Assert.Equal("Updated Title", response.Title);
        Assert.Equal(5, response.EpisodeId);
        Assert.Equal("Updated Director", response.Director);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteMovie_WhenMovieExists()
    {
        var movieRepository = new FakeMovieRepository();
        var unitOfWork = new FakeUnitOfWork();

        var movie = CreateMovie("A New Hope", 4);

        await movieRepository.AddAsync(movie);

        var service = CreateService(
            movieRepository,
            unitOfWork: unitOfWork);

        await service.DeleteAsync(movie.Id);

        Assert.Empty(movieRepository.Movies);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenMovieDoesNotExist()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task SyncFromExternalApiAsync_ShouldCreateMovies_WhenTheyDoNotExist()
    {
        var movieRepository = new FakeMovieRepository();
        var unitOfWork = new FakeUnitOfWork();

        var service = CreateService(
            movieRepository,
            unitOfWork: unitOfWork);

        var response = await service.SyncFromExternalApiAsync();

        Assert.Equal(1, response.Created);
        Assert.Equal(0, response.Updated);
        Assert.Equal(1, response.TotalProcessed);
        Assert.Single(movieRepository.Movies);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task SyncFromExternalApiAsync_ShouldUpdateMovies_WhenTheyAlreadyExist()
    {
        var movieRepository = new FakeMovieRepository();
        var existingMovie = CreateMovie(
            "Old Title",
            4,
            externalId: "1",
            isFromExternalApi: true);

        await movieRepository.AddAsync(existingMovie);

        var service = CreateService(movieRepository);

        var response = await service.SyncFromExternalApiAsync();

        Assert.Equal(0, response.Created);
        Assert.Equal(1, response.Updated);
        Assert.Single(movieRepository.Movies);
        Assert.Equal("A New Hope", movieRepository.Movies.First().Title);
    }

    private static MovieService CreateService(
        FakeMovieRepository? movieRepository = null,
        FakeUnitOfWork? unitOfWork = null)
    {
        return new MovieService(
            movieRepository ?? new FakeMovieRepository(),
            new FakeStarWarsApiClient(),
            new FakeCacheService(),
            unitOfWork ?? new FakeUnitOfWork());
    }

    private static Movie CreateMovie(
        string title,
        int episodeId,
        string? externalId = null,
        bool isFromExternalApi = false)
    {
        return new Movie(
            externalId,
            title,
            episodeId,
            "Opening crawl",
            "George Lucas",
            "Lucasfilm",
            new DateOnly(1977, 5, 25),
            externalId is null ? string.Empty : $"https://www.swapi.tech/api/films/{externalId}",
            isFromExternalApi);
    }
}