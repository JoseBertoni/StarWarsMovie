using StarWarsMovies.Application.Interfaces;
using StarWarsMovies.Domain.Entities;

namespace StarWarsMovies.Tests.Fakes;

public sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    public string GenerateToken(User user, out DateTime expiresAtUtc)
    {
        expiresAtUtc = DateTime.UtcNow.AddHours(1);

        return $"fake-token-for-{user.Email}";
    }
}