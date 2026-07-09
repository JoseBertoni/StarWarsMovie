using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsMovies.Domain.Entities;
using StarWarsMovies.Domain.Enums;

namespace StarWarsMovies.Infrastructure.Persistence.Configurations;

public sealed class MovieExternalResourceConfiguration : IEntityTypeConfiguration<MovieExternalResource>
{
    public void Configure(EntityTypeBuilder<MovieExternalResource> builder)
    {
        builder.ToTable("movie_external_resources");

        builder.HasKey(resource => resource.Id);

        builder.Property(resource => resource.Id)
            .HasColumnName("id");

        builder.Property(resource => resource.MovieId)
            .HasColumnName("movie_id")
            .IsRequired();

        builder.Property(resource => resource.Type)
            .HasColumnName("type")
            .HasConversion(
                type => type.ToString(),
                value => Enum.Parse<MovieExternalResourceType>(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(resource => resource.Url)
            .HasColumnName("url")
            .HasMaxLength(1000)
            .IsRequired();

        builder.HasIndex(resource => new
        {
            resource.MovieId,
            resource.Type,
            resource.Url
        }).IsUnique();
    }
}