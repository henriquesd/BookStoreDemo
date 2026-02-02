# BookStore API

A RESTful Web API for managing a bookstore, built with .NET 10 following Clean Architecture principles.

## Technologies

### Backend
- .NET 10
- Entity Framework Core 10
- SQL Server
- Swagger / OpenAPI

### Testing
- xUnit
- Moq
- FluentAssertions
- AutoFixture
- SQLite In-Memory database

## Architecture

This project follows a **3-layer Clean Architecture** pattern:

```
src/
├── BookStore.Domain/          # Domain Layer - Business Logic
│   ├── Models/                # Entities (Book, Category, OperationResult)
│   ├── Interfaces/            # Repository and Service contracts
│   └── Services/              # Business logic implementation
│
├── BookStore.Infrastructure/  # Infrastructure Layer - Data Access
│   ├── Context/               # EF Core DbContext
│   ├── Repositories/          # Repository implementations
│   └── Mappings/              # Fluent API configurations
│
└── BookStore.API/             # API Layer - HTTP Endpoints
    ├── Controllers/           # RESTful controllers
    ├── Dtos/                  # Request/Response DTOs
    ├── Mappings/              # Model-DTO extension methods
    └── Configuration/         # DI and Swagger setup
```

### Key Patterns

- **Result Pattern**: Services return `IOperationResult<T>` for consistent success/error handling
- **Repository Pattern**: Generic `IRepository<T>` with specialized implementations
- **Manual Mapping**: Extension methods for Model-DTO conversions

## API Endpoints

### Books

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/books` | Get all books |
| GET | `/api/books/GetAllWithPagination?pageNumber=1&pageSize=10` | Get paginated books |
| GET | `/api/books/{id}` | Get book by ID |
| GET | `/api/books/get-books-by-category/{categoryId}` | Get books by category |
| GET | `/api/books/search/{bookName}` | Search books by name |
| GET | `/api/books/search-book-with-category/{searchedValue}` | Search across name, author, description, and category |
| POST | `/api/books` | Create a new book |
| PUT | `/api/books/{id}` | Update a book |
| DELETE | `/api/books/{id}` | Delete a book |

### Categories

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories` | Get all categories |
| GET | `/api/categories/GetAllWithPagination?pageNumber=1&pageSize=10` | Get paginated categories |
| GET | `/api/categories/{id}` | Get category by ID |
| GET | `/api/categories/search/{category}` | Search categories by name |
| POST | `/api/categories` | Create a new category |
| PUT | `/api/categories/{id}` | Update a category |
| DELETE | `/api/categories/{id}` | Delete a category |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or full instance)

### Running the Application

1. Clone the repository
2. Update the connection string in `appsettings.json` if needed
3. Run the migrations:
   ```bash
   dotnet ef database update --project src/BookStore.Infrastructure --startup-project src/BookStore.API
   ```
4. Run the application:
   ```bash
   dotnet run --project src/BookStore.API
   ```
5. Access Swagger UI at `https://localhost:44382/swagger`

## Commands

### Build

```bash
dotnet build BookStore.slnx
```

### Run

```bash
dotnet run --project src/BookStore.API
```

### Run Migrations

```bash
dotnet ef database update --project src/BookStore.Infrastructure --startup-project src/BookStore.API
```

### Create a New Migration

```bash
dotnet ef migrations add <MigrationName> --project src/BookStore.Infrastructure --startup-project src/BookStore.API
```

### Run Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/BookStore.Domain.Tests
dotnet test tests/BookStore.Infrastructure.Tests
dotnet test tests/BookStore.API.Tests
```

## API Testing

Use the `src/BookStore.API/BookStore.http` file for manual API testing with VS Code REST Client or JetBrains Rider HTTP Client.

## Project Structure

```
BookStoreDemo/
├── src/
│   ├── BookStore.API/
│   ├── BookStore.Domain/
│   └── BookStore.Infrastructure/
├── tests/
│   ├── BookStore.API.Tests/
│   ├── BookStore.Domain.Tests/
│   └── BookStore.Infrastructure.Tests/
├── BookStore.slnx
└── Readme.md
```
