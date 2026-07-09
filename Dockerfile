FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore
RUN dotnet publish src/StarWarsMovies.Api/StarWarsMovies.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "StarWarsMovies.Api.dll"]