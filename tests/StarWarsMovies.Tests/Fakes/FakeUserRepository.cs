using StarWarsMovies.Application.Interfaces;
using StarWarsMovies.Domain.Entities;

namespace StarWarsMovies.Tests.Fakes;

public sealed class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    public IReadOnlyCollection<User> Users => _users;

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = _users.FirstOrDefault(user => user.Email == normalizedEmail);

        return Task.FromResult(user);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(user => user.Id == id);

        return Task.FromResult(user);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var exists = _users.Any(user => user.Email == normalizedEmail);

        return Task.FromResult(exists);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.Add(user);

        return Task.CompletedTask;
    }
}