using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using StarWarsMovies.Application.Interfaces;
using StarWarsMovies.Infrastructure.Auth;
using StarWarsMovies.Infrastructure.BackgroundJobs;
using StarWarsMovies.Infrastructure.Cache;
using StarWarsMovies.Infrastructure.ExternalServices.StarWars;
using StarWarsMovies.Infrastructure.Persistence;
using StarWarsMovies.Infrastructure.Repositories;

namespace StarWarsMovies.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.Configure<JwtOptions>(
            configuration.GetSection("Jwt"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<DatabaseSeeder>();

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redisConnectionString = configuration["Redis:ConnectionString"]
                ?? "localhost:6379";

            var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
            redisOptions.AbortOnConnectFail = false;

            return ConnectionMultiplexer.Connect(redisOptions);
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        services.AddHttpClient<IStarWarsApiClient, StarWarsApiClient>(client =>
        {
            var baseUrl = configuration["StarWarsApi:BaseUrl"]
                ?? "https://www.swapi.tech/api/";

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddHostedService<StarWarsMoviesSyncHostedService>();

        return services;
    }
}