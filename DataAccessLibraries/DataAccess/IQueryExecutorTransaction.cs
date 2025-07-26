using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using Dapper;

namespace bgbahasajerman_DataAccessLibrary.DataAccess
{
    /// <summary>
    /// Defines the contract for executing database operations within a transaction.
    /// This interface provides the same functionality as QueryExecutor but operates within a transaction context.
    /// </summary>
    public interface IQueryExecutorTransaction
    {
        /// <summary>
        /// Executes a query that returns a single result within the transaction.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>A single result of type T, or null if not found.</returns>
        Task<T?> QuerySingleAsync<T>(string query, object? parameters = null, int? commandTimeout = null);

        /// <summary>
        /// Executes a query that returns the first result within the transaction.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>The first result of type T, or null if not found.</returns>
        Task<T?> QueryFirstAsync<T>(string query, object? parameters = null, int? commandTimeout = null);

        /// <summary>
        /// Executes a query that returns multiple results within the transaction.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>An enumerable of results of type T.</returns>
        Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null, int? commandTimeout = null);

        /// <summary>
        /// Executes a command that does not return any results within the transaction.
        /// </summary>
        /// <param name="query">The SQL command to execute.</param>
        /// <param name="parameters">The parameters for the command.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>The number of rows affected.</returns>
        Task<int> ExecuteAsync(string query, object? parameters = null, int? commandTimeout = null);

        /// <summary>
        /// Executes a query that returns a single scalar value within the transaction.
        /// </summary>
        /// <typeparam name="T">The type of the scalar result.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>The scalar result of type T.</returns>
        Task<T> ExecuteScalarAsync<T>(string query, object? parameters = null, int? commandTimeout = null);

        /// <summary>
        /// Executes multiple commands in a batch within the transaction.
        /// </summary>
        /// <param name="query">The SQL command to execute.</param>
        /// <param name="parameters">An enumerable of parameter objects, one for each execution.</param>
        /// <param name="commandTimeout">Command timeout in seconds (optional).</param>
        /// <returns>The total number of rows affected.</returns>
        Task<int> ExecuteBatchAsync(string query, IEnumerable<object> parameters, int? commandTimeout = null);
    }
}