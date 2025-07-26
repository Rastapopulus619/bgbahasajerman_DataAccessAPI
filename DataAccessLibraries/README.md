# BgBahasaJerman Data Access Library

A comprehensive, flexible, and reusable .NET 9 data access library for MySQL databases using Dapper. This library provides a clean abstraction layer for database operations with full support for dependency injection, transactions, and type safety.

## Features

- 🏭 **Connection Factory Pattern** - Abstracted database connection management
- 🔄 **Comprehensive CRUD Operations** - Full support for Create, Read, Update, Delete
- 📦 **Type Safety** - Generic methods with compile-time type checking
- 🔒 **Transaction Support** - Built-in transaction management with rollback capabilities
- 🚀 **Async/Await** - Fully asynchronous operations for better performance
- 📊 **Multiple Result Types** - Support for entities, DataTables, scalar values, and more
- 🔧 **Dependency Injection Ready** - Seamless integration with .NET DI container
- 📈 **Batch Operations** - Efficient bulk operations for better performance
- 🛡️ **Error Handling** - Comprehensive error handling examples and patterns

## Quick Start

### 1. Installation

Add the library to your project and ensure you have the required NuGet packages:

```xml
<PackageReference Include="Dapper" Version="2.1.66" />
<PackageReference Include="MySql.Data" Version="9.4.0" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.7" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.7" />
```

### 2. Configuration

Add your connection string to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MySqlConnection": "Server=your-server;Database=your-database;User=your-user;Password=your-password;"
  }
}
```

### 3. Dependency Injection Setup

In your `Program.cs` (or `Startup.cs`):

```csharp
using bgbahasajerman_DataAccessLibrary.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add the data access services
builder.Services.AddDataAccessServices(builder.Configuration);

var app = builder.Build();
```

### 4. Basic Usage

Inject `QueryExecutor` into your services or controllers:

```csharp
public class UserService
{
    private readonly QueryExecutor _queryExecutor;

    public UserService(QueryExecutor queryExecutor)
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<User?> GetUserAsync(int id)
    {
        const string query = "SELECT * FROM Users WHERE Id = @Id";
        return await _queryExecutor.QuerySingleAsync<User>(query, new { Id = id });
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        const string query = "SELECT * FROM Users";
        return await _queryExecutor.QueryAsync<User>(query);
    }

    public async Task<int> CreateUserAsync(User user)
    {
        const string query = @"
            INSERT INTO Users (Username, Email, DateRegistered) 
            VALUES (@Username, @Email, @DateRegistered); 
            SELECT LAST_INSERT_ID();";
        return await _queryExecutor.ExecuteScalarAsync<int>(query, user);
    }
}
```

## Core Methods

### Query Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `QuerySingleAsync<T>()` | Get a single record, throws if multiple found | `T?` |
| `QueryFirstAsync<T>()` | Get first record, doesn't throw if multiple found | `T?` |
| `QueryAsync<T>()` | Get multiple records | `IEnumerable<T>` |
| `QueryAsListAsync<T>()` | Get multiple records as List | `List<T>` |

### Execute Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `ExecuteAsync()` | Execute INSERT/UPDATE/DELETE | `int` (rows affected) |
| `ExecuteBatchAsync()` | Execute multiple commands in batch | `int` (total rows affected) |
| `ExecuteScalarAsync<T>()` | Get single scalar value | `T` |
| `ExecuteScalarOrDefaultAsync<T>()` | Get scalar value with null handling | `T?` |

### Advanced Methods

| Method | Description |
|--------|-------------|
| `QueryAsDataTableAsync()` | Get results as DataTable for dynamic scenarios |
| `QueryMultipleAsync()` | Execute queries with multiple result sets |
| `ExecuteInTransactionAsync()` | Execute operations within a transaction |
| `ExecuteWithConnectionAsync()` | Custom operations with direct connection access |
| `TestConnectionAsync()` | Test database connectivity |

## Examples

### Basic CRUD Operations

```csharp
// Get single user
var user = await _queryExecutor.QuerySingleAsync<User>(
    "SELECT * FROM Users WHERE Id = @Id", 
    new { Id = 1 });

// Get multiple users with filtering
var activeUsers = await _queryExecutor.QueryAsync<User>(
    "SELECT * FROM Users WHERE IsActive = @IsActive", 
    new { IsActive = true });

// Create user and get ID
var userId = await _queryExecutor.ExecuteScalarAsync<int>(
    "INSERT INTO Users (Username, Email) VALUES (@Username, @Email); SELECT LAST_INSERT_ID();",
    new { Username = "john_doe", Email = "john@example.com" });

// Update user
var rowsAffected = await _queryExecutor.ExecuteAsync(
    "UPDATE Users SET Email = @Email WHERE Id = @Id",
    new { Email = "newemail@example.com", Id = 1 });

// Delete user
await _queryExecutor.ExecuteAsync(
    "DELETE FROM Users WHERE Id = @Id",
    new { Id = 1 });
```

### Transactions

```csharp
// Simple transaction
await _queryExecutor.ExecuteInTransactionAsync(async (executor) =>
{
    await executor.ExecuteAsync(
        "INSERT INTO Users (Username) VALUES (@Username)",
        new { Username = "new_user" });
    
    await executor.ExecuteAsync(
        "INSERT INTO Logs (Message) VALUES (@Message)",
        new { Message = "User created" });
});

// Transaction with return value
var newUserId = await _queryExecutor.ExecuteInTransactionAsync(async (executor) =>
{
    var userId = await executor.ExecuteScalarAsync<int>(
        "INSERT INTO Users (Username) VALUES (@Username); SELECT LAST_INSERT_ID();",
        new { Username = "new_user" });

    await executor.ExecuteAsync(
        "INSERT INTO UserProfiles (UserId, Data) VALUES (@UserId, @Data)",
        new { UserId = userId, Data = "profile_data" });

    return userId;
});
```

### Batch Operations

```csharp
// Bulk insert
var users = new[]
{
    new { Username = "user1", Email = "user1@example.com" },
    new { Username = "user2", Email = "user2@example.com" },
    new { Username = "user3", Email = "user3@example.com" }
};

var totalInserted = await _queryExecutor.ExecuteBatchAsync(
    "INSERT INTO Users (Username, Email) VALUES (@Username, @Email)",
    users);
```

### Advanced Scenarios

```csharp
// Multiple result sets
var (users, totalCount) = await _queryExecutor.QueryMultipleAsync(
    "SELECT * FROM Users LIMIT 10; SELECT COUNT(*) FROM Users;",
    async (gridReader) =>
    {
        var userList = (await gridReader.ReadAsync<User>()).ToList();
        var count = await gridReader.ReadSingleAsync<int>();
        return (userList, count);
    });

// Dynamic results with DataTable
var reportData = await _queryExecutor.QueryAsDataTableAsync(
    "SELECT Username, COUNT(*) as PostCount FROM Users u LEFT JOIN Posts p ON u.Id = p.UserId GROUP BY u.Id");

// Scalar operations
var userCount = await _queryExecutor.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users");
var latestDate = await _queryExecutor.ExecuteScalarOrDefaultAsync<DateTime?>("SELECT MAX(DateRegistered) FROM Users");

// Connection testing
var isHealthy = await _queryExecutor.TestConnectionAsync();
```

## Best Practices

### 1. Error Handling

```csharp
try
{
    var user = await _queryExecutor.QuerySingleAsync<User>(query, parameters);
    return user;
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to retrieve user {UserId}", userId);
    throw; // or handle appropriately
}
```

### 2. Using Transactions

Always use transactions for operations that must succeed or fail together:

```csharp
await _queryExecutor.ExecuteInTransactionAsync(async (executor) =>
{
    // Multiple related operations
    await executor.ExecuteAsync(/* operation 1 */);
    await executor.ExecuteAsync(/* operation 2 */);
    // If any operation fails, all will be rolled back
});
```

### 3. Parameter Safety

Always use parameterized queries to prevent SQL injection:

```csharp
// ✅ Good - parameterized
var user = await _queryExecutor.QuerySingleAsync<User>(
    "SELECT * FROM Users WHERE Username = @Username", 
    new { Username = username });

// ❌ Bad - string concatenation (vulnerable to SQL injection)
var user = await _queryExecutor.QuerySingleAsync<User>(
    $"SELECT * FROM Users WHERE Username = '{username}'");
```

### 4. Resource Management

The library automatically handles connection disposal, but for custom operations:

```csharp
await _queryExecutor.ExecuteWithConnectionAsync(async (connection) =>
{
    // Your custom operation
    // Connection will be automatically disposed
});
```

## Architecture

The library follows clean architecture principles:

- **IDbConnectionFactory**: Abstraction for creating database connections
- **MySqlConnectionFactory**: MySQL-specific implementation
- **QueryExecutor**: Main class for database operations
- **IQueryExecutorTransaction**: Interface for transaction operations
- **ServiceCollectionExtensions**: Easy DI registration

## Performance Considerations

- Connection pooling is handled automatically by ADO.NET
- Use batch operations for bulk data operations
- Consider using `QueryAsListAsync()` when you need to enumerate results multiple times
- Transactions have overhead - use them only when necessary
- Use appropriate command timeouts for long-running operations

## Thread Safety

- `QueryExecutor` is thread-safe and can be registered as a singleton or scoped service
- Each operation creates a new connection, ensuring thread safety
- Transactions are isolated per operation

## Contributing

1. Follow the existing code style and patterns
2. Add comprehensive tests for new features
3. Update documentation for any public API changes
4. Ensure all builds pass before submitting PRs

## License

[Add your license information here]