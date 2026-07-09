# Star Wars Movies API

API backend desarrollada en .NET para gestionar películas sincronizadas desde la API pública de Star Wars.

La aplicación incluye autenticación con JWT, autorización por roles, persistencia en PostgreSQL, cache con Redis, sincronización automática en background, documentación con Swagger y tests automatizados.

## Funcionalidades

- Registro e inicio de sesión de usuarios
- Autenticación con JWT
- Autorización basada en roles
- Roles de usuario: Admin y Regular
- CRUD de películas
- Sincronización con la API pública de Star Wars
- Sincronización inicial automática al levantar la aplicación
- Persistencia en PostgreSQL usando Entity Framework Core Code First
- Cache con Redis para consultas de películas
- Middleware global para manejo de errores
- Documentación con Swagger y soporte para Bearer Token
- Docker Compose para infraestructura local
- Tests unitarios con xUnit

## Tecnologías utilizadas

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Redis
- JWT Bearer Authentication
- Swagger / Swashbuckle
- xUnit
- Docker Compose

## Arquitectura

El proyecto sigue una arquitectura basada en Clean Architecture:

```txt
src/
  StarWarsMovies.Api
  StarWarsMovies.Application
  StarWarsMovies.Domain
  StarWarsMovies.Infrastructure

tests/
  StarWarsMovies.Tests
```

### Responsabilidades por capa

```txt
Domain:
- Entidades principales
- Enums
- Reglas básicas de dominio dentro de las entidades

Application:
- DTOs
- Interfaces
- Servicios de aplicación
- Casos de uso

Infrastructure:
- DbContext de Entity Framework Core
- Repositories
- Persistencia con PostgreSQL
- Cache con Redis
- Generación de JWT
- Hash de contraseñas
- Cliente HTTP para Star Wars API
- Background jobs

API:
- Controllers
- Configuración de autenticación
- Configuración de Swagger
- Middleware global de errores
```

## Requisitos

Antes de ejecutar el proyecto, asegurarse de tener instalado:

- .NET SDK 10
- Docker Desktop
- dotnet-ef

Instalar la herramienta de Entity Framework Core:

```bash
dotnet tool install --global dotnet-ef
```

O actualizarla si ya está instalada:

```bash
dotnet tool update --global dotnet-ef
```

## Ejecución local

### 1. Levantar PostgreSQL y Redis

Desde la raíz del proyecto:

```bash
docker compose up -d
```

Esto levantará:

```txt
PostgreSQL: localhost:5432
Redis:      localhost:6379
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Compilar la solución

```bash
dotnet build
```

### 4. Aplicar migrations

```bash
dotnet ef database update -p src/StarWarsMovies.Infrastructure -s src/StarWarsMovies.Api
```

### 5. Ejecutar la API

```bash
dotnet run --project src/StarWarsMovies.Api --urls "http://localhost:5000"
```

Swagger estará disponible en:

```txt
http://localhost:5000/swagger
```

## Usuarios por defecto

La aplicación crea dos usuarios por defecto al iniciar.

### Usuario Admin

```json
{
  "email": "admin@test.com",
  "password": "Admin123!"
}
```

### Usuario Regular

```json
{
  "email": "user@test.com",
  "password": "User123!"
}
```

## Flujo de autenticación

1. Abrir Swagger.
2. Ejecutar `POST /api/auth/login`.
3. Copiar el valor de `accessToken`.
4. Hacer click en el botón `Authorize` de Swagger.
5. Pegar el token usando el siguiente formato:

```txt
Bearer TU_ACCESS_TOKEN
```

## Roles y permisos

### Endpoints públicos

```txt
POST /api/auth/sign-up
POST /api/auth/login
```

### Usuarios autenticados

```txt
GET /api/movies
```

### Solo usuarios Regular

```txt
GET /api/movies/{id}
```

### Solo usuarios Admin

```txt
POST /api/movies
PUT /api/movies/{id}
DELETE /api/movies/{id}
POST /api/movies/sync
```

## Endpoints principales

### Auth

```txt
POST /api/auth/sign-up
POST /api/auth/login
```

### Movies

```txt
GET    /api/movies
GET    /api/movies/{id}
POST   /api/movies
PUT    /api/movies/{id}
DELETE /api/movies/{id}
POST   /api/movies/sync
```

## Sincronización con Star Wars API

La aplicación sincroniza películas desde la API pública de Star Wars.

La sincronización ocurre de dos formas:

```txt
1. Automáticamente al iniciar la aplicación mediante un BackgroundService
2. Manualmente mediante POST /api/movies/sync, disponible solo para usuarios Admin
```

El proceso de sincronización guarda la información en PostgreSQL y actualiza la cache de Redis cuando corresponde.

Esto permite que la API pueda seguir respondiendo con información local incluso si la API externa deja de estar disponible temporalmente.

## Estrategia de cache

La API utiliza Redis para cachear consultas de películas.

Estrategia actual:

```txt
GET /api/movies
- Intenta obtener la información desde Redis
- Si no existe en cache, consulta PostgreSQL
- Guarda la respuesta en Redis

GET /api/movies/{id}
- Intenta obtener la información desde Redis
- Si no existe en cache, consulta PostgreSQL
- Guarda la respuesta en Redis
```

Si Redis no está disponible, la aplicación registra un warning y continúa funcionando con PostgreSQL.

## Base de datos

El proyecto utiliza PostgreSQL con Entity Framework Core Code First.

Tablas principales:

```txt
users
movies
movie_external_resources
```

La tabla `movie_external_resources` guarda referencias externas provenientes de la API de Star Wars, como:

```txt
characters
planets
species
starships
vehicles
```

Esta decisión mantiene el dominio enfocado en la gestión de películas sin sobre-modelar toda la API externa.

## Tests

El proyecto incluye tests unitarios usando xUnit.

Para ejecutar los tests:

```bash
dotnet test
```

Áreas cubiertas:

```txt
AuthService:
- Registro de usuarios
- Validación de email duplicado
- Login
- Credenciales inválidas

MovieService:
- Creación de película
- Actualización de película
- Eliminación de película
- Obtener listado de películas
- Obtener detalle de película
- Escenarios de recurso no encontrado
- Sincronización desde API externa
```

## Manejo de errores

La API utiliza un middleware global de errores para devolver respuestas consistentes.

Escenarios manejados:

```txt
400 Bad Request
401 Unauthorized
404 Not Found
409 Conflict
500 Internal Server Error
```

Ejemplo de respuesta de error:

```json
{
  "status": 404,
  "title": "Resource not found",
  "detail": "Movie with key '...' was not found.",
  "errors": null,
  "traceId": "..."
}
```

## Decisiones técnicas

### Clean Architecture

La solución está separada en Domain, Application, Infrastructure, API y Tests para mantener responsabilidades claras y mejorar la mantenibilidad.

### Repository Pattern

Los repositories abstraen el acceso a datos desde los servicios de aplicación.

### Unit of Work

El Unit of Work coordina la persistencia de cambios utilizando Entity Framework Core.

### Redis Cache

Redis se utiliza como mejora de performance y disponibilidad. Si Redis falla, la aplicación continúa respondiendo desde PostgreSQL.

### Background Synchronization

Se implementó un BackgroundService que sincroniza películas de Star Wars al iniciar la aplicación. Esto permite contar con información local sin depender en cada request de la API externa.

### Aislamiento de API externa

Los modelos de respuesta de Star Wars API se encuentran dentro de la capa Infrastructure. La capa Domain no depende del contrato externo.

### Autenticación y autorización

Se utiliza JWT para autenticación y autorización basada en roles para restringir operaciones sensibles a usuarios Admin.

## Configuración de entorno

La configuración local se encuentra en:

```txt
src/StarWarsMovies.Api/appsettings.json
```

Para ambientes productivos, secretos como la key de JWT y credenciales de base de datos deberían moverse a variables de entorno o a un secret manager.

## Servicios Docker

El entorno local utiliza Docker Compose con:

```txt
PostgreSQL
Redis
```

La configuración está definida en:

```txt
docker-compose.yml
```