using StarWarsMovies.Application.Common.Exceptions;
using StarWarsMovies.Application.DTOs.Auth;
using StarWarsMovies.Application.Services;
using StarWarsMovies.Domain.Entities;
using StarWarsMovies.Domain.Enums;
using StarWarsMovies.Tests.Fakes;

namespace StarWarsMovies.Tests.Application;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task SignUpAsync_ShouldCreateRegularUser_WhenRequestIsValid()
    {
        var userRepository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var jwtTokenGenerator = new FakeJwtTokenGenerator();
        var unitOfWork = new FakeUnitOfWork();

        var service = new AuthService(
            userRepository,
            passwordHasher,
            jwtTokenGenerator,
            unitOfWork);

        var request = new SignUpRequest(
            "test@email.com",
            "Password123!");

        var response = await service.SignUpAsync(request);

        Assert.NotEqual(Guid.Empty, response.UserId);
        Assert.Equal("test@email.com", response.Email);
        Assert.Equal(UserRole.Regular.ToString(), response.Role);
        Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
        Assert.Single(userRepository.Users);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task SignUpAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
    {
        var userRepository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();

        await userRepository.AddAsync(new User(
            "test@email.com",
            passwordHasher.Hash("Password123!"),
            UserRole.Regular));

        var service = new AuthService(
            userRepository,
            passwordHasher,
            new FakeJwtTokenGenerator(),
            new FakeUnitOfWork());

        var request = new SignUpRequest(
            "test@email.com",
            "Password123!");

        await Assert.ThrowsAsync<ConflictException>(() =>
            service.SignUpAsync(request));
    }

    [Fact]
    public async Task SignUpAsync_ShouldThrowValidationException_WhenEmailIsInvalid()
    {
        var service = new AuthService(
            new FakeUserRepository(),
            new FakePasswordHasher(),
            new FakeJwtTokenGenerator(),
            new FakeUnitOfWork());

        var request = new SignUpRequest(
            "invalid-email",
            "Password123!");

        await Assert.ThrowsAsync<ValidationException>(() =>
            service.SignUpAsync(request));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var userRepository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();

        await userRepository.AddAsync(new User(
            "admin@test.com",
            passwordHasher.Hash("Admin123!"),
            UserRole.Admin));

        var service = new AuthService(
            userRepository,
            passwordHasher,
            new FakeJwtTokenGenerator(),
            new FakeUnitOfWork());

        var request = new LoginRequest(
            "admin@test.com",
            "Admin123!");

        var response = await service.LoginAsync(request);

        Assert.Equal("admin@test.com", response.Email);
        Assert.Equal(UserRole.Admin.ToString(), response.Role);
        Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenPasswordIsInvalid()
    {
        var userRepository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();

        await userRepository.AddAsync(new User(
            "admin@test.com",
            passwordHasher.Hash("Admin123!"),
            UserRole.Admin));

        var service = new AuthService(
            userRepository,
            passwordHasher,
            new FakeJwtTokenGenerator(),
            new FakeUnitOfWork());

        var request = new LoginRequest(
            "admin@test.com",
            "WrongPassword!");

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            service.LoginAsync(request));
    }
}