using StarWarsMovies.Domain.Enums;

namespace StarWarsMovies.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public UserRole Role { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private User()
    {
    }

    public User(string email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        Id = Guid.NewGuid();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
    }
}