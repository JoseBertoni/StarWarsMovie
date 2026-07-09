using Microsoft.EntityFrameworkCore;
using StarWarsMovies.Domain.Entities;

namespace StarWarsMovies.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Movie> Movies => Set<Movie>();

    public DbSet<MovieExternalResource> MovieExternalResources => Set<MovieExternalResource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}