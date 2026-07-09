using StarWarsMovies.Application.DTOs.Auth;

namespace StarWarsMovies.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}