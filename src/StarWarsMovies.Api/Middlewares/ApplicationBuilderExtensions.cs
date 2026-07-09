using StarWarsMovies.Api.Middlewares;

namespace StarWarsMovies.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        return app;
    }
}