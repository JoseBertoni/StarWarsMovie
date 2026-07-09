using System.Net.Mail;
using StarWarsMovies.Application.Common.Exceptions;
using StarWarsMovies.Application.DTOs.Auth;
using StarWarsMovies.Application.Interfaces;
using StarWarsMovies.Domain.Entities;
using StarWarsMovies.Domain.Enums;

namespace StarWarsMovies.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default)
    {
        ValidateSignUpRequest(request);

        var normalizedEmail = NormalizeEmail(request.Email);

        var exists = await _userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken);

        if (exists)
            throw new ConflictException("A user with the same email already exists.");

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User(
            normalizedEmail,
            passwordHash,
            UserRole.Regular);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user, out var expiresAtUtc);

        return new AuthResponse(
            user.Id,
            user.Email,
            user.Role.ToString(),
            token,
            expiresAtUtc);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ValidateLoginRequest(request);

        var normalizedEmail = NormalizeEmail(request.Email);

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
            throw new UnauthorizedException("Invalid email or password.");

        var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
            throw new UnauthorizedException("Invalid email or password.");

        var token = _jwtTokenGenerator.GenerateToken(user, out var expiresAtUtc);

        return new AuthResponse(
            user.Id,
            user.Email,
            user.Role.ToString(),
            token,
            expiresAtUtc);
    }

    private static void ValidateSignUpRequest(SignUpRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
            errors["email"] = ["A valid email is required."];

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors["password"] = ["Password is required."];
        }
        else if (request.Password.Length < 8)
        {
            errors["password"] = ["Password must contain at least 8 characters."];
        }

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }

    private static void ValidateLoginRequest(LoginRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Email))
            errors["email"] = ["Email is required."];

        if (string.IsNullOrWhiteSpace(request.Password))
            errors["password"] = ["Password is required."];

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}