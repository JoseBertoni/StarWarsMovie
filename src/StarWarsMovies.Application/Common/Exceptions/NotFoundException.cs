namespace StarWarsMovies.Application.Common.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with key '{key}' was not found.")
    {
    }
}