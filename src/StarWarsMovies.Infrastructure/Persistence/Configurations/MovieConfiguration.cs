using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsMovies.Domain.Entities;

namespace StarWarsMovies.Infrastructure.Persistence.Configurations;

public sealed class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("movies");

        builder.HasKey(movie => movie.Id);

        builder.Property(movie => movie.Id)
            .HasColumnName("id");

        builder.Property(movie => movie.ExternalId)
            .HasColumnName("external_id")
            .HasMaxLength(100);

        builder.HasIndex(movie => movie.ExternalId)
            .IsUnique();

        builder.Property(movie => movie.Title)
            .HasColumnName("title")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(movie => movie.EpisodeId)
            .HasColumnName("episode_id")
            .IsRequired();

        builder.Property(movie => movie.OpeningCrawl)
            .HasColumnName("opening_crawl")
            .IsRequired();

        builder.Property(movie => movie.Director)
            .HasColumnName("director")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(movie => movie.Producer)
            .HasColumnName("producer")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(movie => movie.ReleaseDate)
            .HasColumnName("release_date")
            .IsRequired();

        builder.Property(movie => movie.SourceUrl)
            .HasColumnName("source_url")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(movie => movie.IsFromExternalApi)
            .HasColumnName("is_from_external_api")
            .IsRequired();

        builder.Property(movie => movie.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(movie => movie.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        builder.HasMany(movie => movie.ExternalResources)
            .WithOne(resource => resource.Movie)
            .HasForeignKey(resource => resource.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}