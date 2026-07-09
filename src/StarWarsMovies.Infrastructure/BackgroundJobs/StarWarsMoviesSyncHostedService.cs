using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StarWarsMovies.Application.Interfaces;

namespace StarWarsMovies.Infrastructure.BackgroundJobs;

public sealed class StarWarsMoviesSyncHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StarWarsMoviesSyncHostedService> _logger;

    public StarWarsMoviesSyncHostedService(
        IServiceProvider serviceProvider,
        ILogger<StarWarsMoviesSyncHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        try
        {
            using var scope = _serviceProvider.CreateScope();

            var movieService = scope.ServiceProvider.GetRequiredService<IMovieService>();

            var result = await movieService.SyncFromExternalApiAsync(stoppingToken);

            _logger.LogInformation(
                "Initial Star Wars movies sync completed. Created: {Created}, Updated: {Updated}, TotalProcessed: {TotalProcessed}",
                result.Created,
                result.Updated,
                result.TotalProcessed);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Initial Star Wars movies sync was cancelled.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Initial Star Wars movies sync failed.");
        }
    }
}