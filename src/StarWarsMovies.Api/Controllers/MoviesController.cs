using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarWarsMovies.Application.DTOs.Movies;
using StarWarsMovies.Application.Interfaces;
using StarWarsMovies.Domain.Enums;

namespace StarWarsMovies.Api.Controllers;

[ApiController]
[Route("api/movies")]
[Authorize]
public sealed class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<MovieSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<MovieSummaryResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var movies = await _movieService.GetAllAsync(cancellationToken);

        return Ok(movies);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Regular))]
    [ProducesResponseType(typeof(MovieDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetailResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var movie = await _movieService.GetByIdAsync(id, cancellationToken);

        return Ok(movie);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(MovieDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MovieDetailResponse>> Create(
        [FromBody] CreateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var movie = await _movieService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = movie.Id },
            movie);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(MovieDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetailResponse>> Update(
        Guid id,
        [FromBody] UpdateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var movie = await _movieService.UpdateAsync(id, request, cancellationToken);

        return Ok(movie);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _movieService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    [HttpPost("sync")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(SyncMoviesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SyncMoviesResponse>> Sync(
        CancellationToken cancellationToken)
    {
        var result = await _movieService.SyncFromExternalApiAsync(cancellationToken);

        return Ok(result);
    }
}