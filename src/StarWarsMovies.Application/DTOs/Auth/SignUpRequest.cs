namespace StarWarsMovies.Application.DTOs.Auth;

public sealed record SignUpRequest(
    string Email,
    string Password);