# BookStore
Web API Project using .NET 10

## Technologies
- .NET 10
- Entity Framework Core
- Fluent API
- AutoMapper
- Swagger
- SQL Server

### Unit tests
- xUnit
- Moq
- Fluent Assertions
- AutoFixture
- SQLite In-Memory database

## Architecture
- 3 Layers:
  - Application layer (API)
    - Controllers
    - Dtos
  - Domain layer
    - Models
    - Interfaces
    - Services
  - Infrastructure layer
    - Repository Pattern

# Commands

## Run migration
`dotnet ef database update --project src/BookStore.Infrastructure --startup-project src/BookStore.API`