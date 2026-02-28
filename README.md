# BookStore API

A RESTful Web API for managing a bookstore, built with .NET 10 following Clean Architecture principles and modern best practices.

## Technologies

### Backend
- .NET 10
- Entity Framework Core 10
- SQL Server
- Swagger / OpenAPI

### Testing
- xUnit
- NSubstitute
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
│   ├── Services/              # Business logic implementation
│   └── Constants/             # Error messages and constants
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
    ├── Middleware/            # Global exception handling
    └── Configuration/         # DI and Swagger setup
```

### Key Patterns & Features

- **Result Pattern**: All service methods return `IOperationResult<T>` for consistent success/error handling
- **Repository Pattern**: Generic `IRepository<T>` with specialized implementations
- **Manual Mapping**: Extension methods for Model-DTO conversions
- **Standardized Error Responses**: `ErrorResponse` DTO for consistent API errors
- **Structured Logging**: Comprehensive logging with `ILogger<T>` using structured parameters
- **N+1 Query Prevention**: All queries use eager loading with `.Include()` to prevent N+1 issues
- **Search Pagination**: All search operations support pagination to handle large result sets efficiently

## API Endpoints

### Books

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/books` | Get all books |
| GET | `/api/books/pagination?pageNumber=1&pageSize=10` | Get paginated books (max 100 per page) |
| GET | `/api/books/{id}` | Get book by ID |
| GET | `/api/books/categories/{categoryId}` | Get books by category |
| GET | `/api/books/search?q={term}` | Search books by name (returns all matches) |
| GET | `/api/books/search/pagination?q={term}&pageNumber=1&pageSize=10` | Search books by name with pagination |
| GET | `/api/books/search-with-category?q={term}` | Search across name, author, description, and category (returns all matches) |
| GET | `/api/books/search-with-category/pagination?q={term}&pageNumber=1&pageSize=10` | Search with category filter with pagination |
| POST | `/api/books` | Create a new book |
| PUT | `/api/books/{id}` | Update a book |
| DELETE | `/api/books/{id}` | Delete a book |

### Categories

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories` | Get all categories |
| GET | `/api/categories/pagination?pageNumber=1&pageSize=10` | Get paginated categories (max 100 per page) |
| GET | `/api/categories/{id}` | Get category by ID |
| GET | `/api/categories/search?q={term}` | Search categories by name (returns all matches) |
| GET | `/api/categories/search/pagination?q={term}&pageNumber=1&pageSize=10` | Search categories by name with pagination |
| POST | `/api/categories` | Create a new category |
| PUT | `/api/categories/{id}` | Update a category |
| DELETE | `/api/categories/{id}` | Delete a category |

### API Design Notes

- **Kebab-case routes**: All routes use lowercase with hyphens (e.g., `/search-with-category`)
- **Query parameters**: Search operations use `?q=` query parameter instead of route parameters
- **Pagination support**:
  - Standard pagination: `/pagination?pageNumber=1&pageSize=10`
  - Search pagination: `/search/pagination?q={term}&pageNumber=1&pageSize=10`
  - Page size is validated between 1-100 using `[Range]` attributes
  - Returns `PagedResponse<T>` with metadata (TotalPages, TotalRecords, PageNumber, PageSize)
- **Consistent error codes**:
  - `200 OK` - Success
  - `201 Created` - Resource created
  - `204 No Content` - Successful delete
  - `400 Bad Request` - Validation error
  - `404 Not Found` - Resource not found
  - `409 Conflict` - Duplicate resource or has dependencies
  - `500 Internal Server Error` - Unexpected error

## Response Formats

### Paginated Response

Pagination endpoints return a `PagedResponse<T>` structure:

```json
{
  "pageNumber": 1,
  "pageSize": 10,
  "totalRecords": 42,
  "totalPages": 5,
  "data": [
    {
      "id": 1,
      "name": "Book Title",
      "author": "Author Name",
      "value": 29.99,
      "publishDate": "2024-01-15T00:00:00",
      "categoryId": 1,
      "category": {
        "id": 1,
        "name": "Fiction"
      }
    }
  ]
}
```

### Error Response Format

All error responses follow a consistent structure:

```json
{
  "message": "Validation error message"
}
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or full instance)

### Running the Application

1. Clone the repository
   ```bash
   git clone https://github.com/henriquesd/BookStoreDemo.git
   cd BookStoreDemo
   ```

2. Update the connection string in `src/BookStore.API/appsettings.json` if needed

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
# Run all tests (240 tests)
dotnet test

# Run specific test project
dotnet test tests/BookStore.Domain.Tests
dotnet test tests/BookStore.Infrastructure.Tests
dotnet test tests/BookStore.API.Tests

# Run tests with detailed output
dotnet test --verbosity detailed
```

## API Testing

Use the `src/BookStore.API/BookStore.http` file for manual API testing with:
- **VS Code**: Install REST Client extension
- **JetBrains Rider**: Built-in HTTP Client
- **Visual Studio**: Built-in .http file support

The file includes comprehensive test scenarios for:
- Happy path requests
- Validation error tests
- Not found scenarios
- Edge cases

## Project Structure

```
BookStoreDemo/
├── src/
│   ├── BookStore.API/              # HTTP API Layer
│   │   ├── Controllers/            # API Controllers
│   │   ├── Dtos/                   # Data Transfer Objects
│   │   ├── Mappings/               # Extension methods for mapping
│   │   ├── Middleware/             # Exception handling middleware
│   │   ├── Configuration/          # DI and service configuration
│   │   ├── GlobalUsings.cs         # Global using directives
│   │   └── BookStore.http          # API test collection
│   │
│   ├── BookStore.Domain/           # Domain Layer
│   │   ├── Models/                 # Domain entities
│   │   ├── Interfaces/             # Contracts
│   │   ├── Services/               # Business logic
│   │   └── Constants/              # Error messages
│   │
│   └── BookStore.Infrastructure/   # Data Access Layer
│       ├── Context/                # DbContext
│       ├── Repositories/           # Repository implementations
│       ├── Mappings/               # EF Core configurations
│       └── Migrations/             # Database migrations
│
├── tests/
│   ├── BookStore.API.Tests/        # Controller integration tests
│   ├── BookStore.Domain.Tests/     # Service unit tests
│   └── BookStore.Infrastructure.Tests/  # Repository tests
│
├── BookStore.slnx                  # Solution file
└── README.md                       # This file
```
