using StarWarsMovies.Domain.Entities;

namespace StarWarsMovies.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, out DateTime expiresAtUtc);
}