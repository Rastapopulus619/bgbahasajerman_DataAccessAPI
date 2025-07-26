using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using IsolationLevel = System.Data.IsolationLevel;

namespace bgbahasajerman_DataAccessLibrary.DataAccess
{
    /// <summary>
    /// A comprehensive query executor for database operations using Dapper.
    /// This class provides various methods for executing different types of database operations
    /// while abstracting away connection management and ensuring type safety.
    /// 
    /// USAGE EXAMPLES:
    /// 
    /// 1. Get a single entity:
    ///    var user = await queryExecutor.QuerySingleAsync&lt;User&gt;("SELECT * FROM Users WHERE Id = @Id", new { Id = 1 });
    /// 
    /// 2. Get multiple entities:
    ///    var users = await queryExecutor.QueryAsync&lt;User&gt;("SELECT * FROM Users WHERE IsActive = @IsActive", new { IsActive = true });
    /// 
    /// 3. Execute a command (INSERT/UPDATE/DELETE):
    ///    var rowsAffected = await queryExecutor.ExecuteAsync("UPDATE Users SET Email = @Email WHERE Id = @Id", new { Email = "new@email.com", Id = 1 });
    /// 
    /// 4. Execute with transaction:
    ///    await queryExecutor.ExecuteInTransactionAsync(async (executor) => {
    ///        await executor.ExecuteAsync("INSERT INTO Users (Name) VALUES (@Name)", new { Name = "John" });
    ///        await executor.ExecuteAsync("INSERT INTO Logs (Message) VALUES (@Message)", new { Message = "User created" });
    ///    });
    /// 
    /// 5. Get scalar value:
    ///    var count = await queryExecutor.ExecuteScalarAsync&lt;int&gt;("SELECT COUNT(*) FROM Users");
    /// 
    /// 6. Get results as DataTable (useful for dynamic scenarios):
    ///    var dataTable = await queryExecutor.QueryAsDataTableAsync("SELECT * FROM Users");
    /// </summary>
    public class QueryExecutor
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        /// <summary>
        /// Initializes a new instance of the QueryExecutor class.
        /// </summary>
        /// <param name="dbConnectionFactory">The factory to create database connections.</param>
        public QueryExecutor(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
        }

        #region Basic Query Methods

        /// <summary>
        /// Executes a query that returns a single result.
        /// Returns null if no record is found.
        /// Throws an exception if more than one record is found.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>A single result of type T, or null if not found.</returns>
        public async Task<T?> QuerySingleAsync<T>(string query, object? parameters = null, int? commandTimeout = null)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(query, parameters, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// Executes a query that returns the first result or null if no results are found.
        /// Does not throw an exception if multiple records are found (unlike QuerySingleAsync).
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>The first result of type T, or null if not found.</returns>
        public async Task<T?> QueryFirstAsync<T>(string query, object? parameters = null, int? commandTimeout = null)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(query, parameters, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// Executes a query that returns multiple results.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>An enumerable of results of type T.</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null, int? commandTimeout = null)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryAsync<T>(query, parameters, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// Executes a query and returns the results as a List for easier manipulation.
        /// Useful when you need to modify the collection or access it multiple times.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>A list of results of type T.</returns>
        public async Task<List<T>> QueryAsListAsync<T>(string query, object? parameters = null, int? commandTimeout = null)
        {
            var results = await QueryAsync<T>(query, parameters, commandTimeout);
            return results.ToList();
        }

        #endregion

        #region Execute Methods (Non-Query)

        /// <summary>
        /// Executes a command that does not return any results (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="query">The SQL command to execute.</param>
        /// <param name="parameters">The parameters for the command.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> ExecuteAsync(string query, object? parameters = null, int? commandTimeout = null)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.ExecuteAsync(query, parameters, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// Executes multiple commands in a batch for better performance.
        /// Useful for bulk operations like inserting many records.
        /// </summary>
        /// <param name="query">The SQL command to execute.</param>
        /// <param name="parameters">An enumerable of parameter objects, one for each execution.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>The total number of rows affected.</returns>
        public async Task<int> ExecuteBatchAsync(string query, IEnumerable<object> parameters, int? commandTimeout = null)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.ExecuteAsync(query, parameters, commandTimeout: commandTimeout);
        }

        #endregion

        #region Scalar Methods

        /// <summary>
        /// Executes a query that returns a single scalar value (e.g., COUNT, SUM, MAX).
        /// </summary>
        /// <typeparam name="T">The type of the scalar result.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>The scalar result of type T.</returns>
        public async Task<T> ExecuteScalarAsync<T>(string query, object? parameters = null, int? commandTimeout = null)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<T>(query, parameters, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// Executes a query that returns a single scalar value, with null handling.
        /// Returns null if the database returns NULL or no results.
        /// </summary>
        /// <typeparam name="T">The type of the scalar result.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>The scalar result of type T, or null.</returns>
        public async Task<T?> ExecuteScalarOrDefaultAsync<T>(string query, object? parameters = null, int? commandTimeout = null)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var result = await connection.ExecuteScalarAsync(query, parameters, commandTimeout: commandTimeout);
            
            if (result == null || result == DBNull.Value)
                return default(T);
                
            return (T)Convert.ChangeType(result, typeof(T));
        }

        #endregion

        #region DataTable Methods

        /// <summary>
        /// Executes a query and returns the results as a DataTable.
        /// Useful for dynamic scenarios where you don't have a specific type,
        /// or when working with reporting tools that expect DataTable.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>A DataTable containing the query results.</returns>
        public async Task<DataTable> QueryAsDataTableAsync(string query, object? parameters = null, int? commandTimeout = null)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var reader = await connection.ExecuteReaderAsync(query, parameters, commandTimeout: commandTimeout);
            
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }

        #endregion

        #region Multi-Result Methods

        /// <summary>
        /// Executes a query that returns multiple result sets.
        /// Useful for stored procedures or queries with multiple SELECT statements.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>A GridReader for accessing multiple result sets.</returns>
        public async Task<SqlMapper.GridReader> QueryMultipleAsync(string query, object? parameters = null, int? commandTimeout = null)
        {
            var connection = _dbConnectionFactory.CreateConnection();
            // Note: Connection will be disposed when GridReader is disposed
            return await connection.QueryMultipleAsync(query, parameters, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// Executes a query with multiple result sets and processes them with a provided function.
        /// This ensures proper disposal of connections and GridReader.
        /// </summary>
        /// <typeparam name="T">The return type of the processing function.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="processor">Function to process the GridReader and return results.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>The result of the processing function.</returns>
        public async Task<T> QueryMultipleAsync<T>(string query, Func<SqlMapper.GridReader, Task<T>> processor, object? parameters = null, int? commandTimeout = null)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            using var gridReader = await connection.QueryMultipleAsync(query, parameters, commandTimeout: commandTimeout);
            return await processor(gridReader);
        }

        #endregion

        #region Transaction Methods

        /// <summary>
        /// Executes multiple database operations within a single transaction.
        /// If any operation fails, all operations are rolled back.
        /// </summary>
        /// <param name="operations">A function that performs multiple database operations.</param>
        /// <param name="isolationLevel">The transaction isolation level (optional).</param>
        /// <returns>A task representing the async operation.</returns>
        public async Task ExecuteInTransactionAsync(Func<IQueryExecutorTransaction, Task> operations, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();
            
            using var transaction = connection.BeginTransaction(isolationLevel);
            var transactionExecutor = new QueryExecutorTransaction(connection, transaction);
            
            try
            {
                await operations(transactionExecutor);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Executes multiple database operations within a single transaction and returns a result.
        /// If any operation fails, all operations are rolled back.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="operations">A function that performs multiple database operations and returns a result.</param>
        /// <param name="isolationLevel">The transaction isolation level (optional).</param>
        /// <returns>The result of the operations.</returns>
        public async Task<T> ExecuteInTransactionAsync<T>(Func<IQueryExecutorTransaction, Task<T>> operations, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();
            
            using var transaction = connection.BeginTransaction(isolationLevel);
            var transactionExecutor = new QueryExecutorTransaction(connection, transaction);
            
            try
            {
                var result = await operations(transactionExecutor);
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Executes a custom operation with direct access to the database connection.
        /// Use this for scenarios not covered by the standard methods.
        /// The connection is automatically opened and disposed.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="operation">The operation to perform with the connection.</param>
        /// <returns>The result of the operation.</returns>
        public async Task<T> ExecuteWithConnectionAsync<T>(Func<IDbConnection, Task<T>> operation)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();
            return await operation(connection);
        }

        /// <summary>
        /// Executes a custom operation with direct access to the database connection.
        /// Use this for scenarios not covered by the standard methods.
        /// The connection is automatically opened and disposed.
        /// </summary>
        /// <param name="operation">The operation to perform with the connection.</param>
        /// <returns>A task representing the async operation.</returns>
        public async Task ExecuteWithConnectionAsync(Func<IDbConnection, Task> operation)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();
            await operation(connection);
        }

        /// <summary>
        /// Tests the database connection by executing a simple query.
        /// Useful for health checks or connection validation.
        /// </summary>
        /// <returns>True if the connection is working, false otherwise.</returns>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = _dbConnectionFactory.CreateConnection();
                connection.Open();
                await connection.ExecuteScalarAsync("SELECT 1");
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
