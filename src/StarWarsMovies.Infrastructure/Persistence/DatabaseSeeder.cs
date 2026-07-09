using Microsoft.EntityFrameworkCore;
using StarWarsMovies.Application.Interfaces;
using StarWarsMovies.Domain.Entities;
using StarWarsMovies.Domain.Enums;

namespace StarWarsMovies.Infrastructure.Persistence;

public sealed class DatabaseSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(
        AppDbContext dbContext,
        IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedAdminUserAsync(cancellationToken);
        await SeedRegularUserAsync(cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedAdminUserAsync(CancellationToken cancellationToken)
    {
        const string email = "admin@test.com";

        var exists = await _dbContext.Users
            .AnyAsync(user => user.Email == email, cancellationToken);

        if (exists)
            return;

        var admin = new User(
            email,
            _passwordHasher.Hash("Admin123!"),
            UserRole.Admin);

        await _dbContext.Users.AddAsync(admin, cancellationToken);
    }

    private async Task SeedRegularUserAsync(CancellationToken cancellationToken)
    {
        const string email = "user@test.com";

        var exists = await _dbContext.Users
            .AnyAsync(user => user.Email == email, cancellationToken);

        if (exists)
            return;

        var user = new User(
            email,
            _passwordHasher.Hash("User123!"),
            UserRole.Regular);

        await _dbContext.Users.AddAsync(user, cancellationToken);
    }
}