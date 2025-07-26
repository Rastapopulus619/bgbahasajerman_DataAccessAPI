using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace bgbahasajerman_DataAccessLibrary.DataAccess
{
    /// <summary>
    /// Implementation of IQueryExecutorTransaction that executes database operations within a transaction.
    /// This class provides the same functionality as QueryExecutor but ensures all operations
    /// are executed within the provided transaction context.
    /// </summary>
    internal class QueryExecutorTransaction : IQueryExecutorTransaction
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        /// <summary>
        /// Initializes a new instance of the QueryExecutorTransaction class.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="transaction">The transaction to execute operations within.</param>
        public QueryExecutorTransaction(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        /// <inheritdoc />
        public async Task<T?> QuerySingleAsync<T>(string query, object? parameters = null, int? commandTimeout = null)
        {
            return await _connection.QuerySingleOrDefaultAsync<T>(query, parameters, _transaction, commandTimeout);
        }

        /// <inheritdoc />
        public async Task<T?> QueryFirstAsync<T>(string query, object? parameters = null, int? commandTimeout = null)
        {
            return await _connection.QueryFirstOrDefaultAsync<T>(query, parameters, _transaction, commandTimeout);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null, int? commandTimeout = null)
        {
            return await _connection.QueryAsync<T>(query, parameters, _transaction, commandTimeout);
        }

        /// <inheritdoc />
        public async Task<int> ExecuteAsync(string query, object? parameters = null, int? commandTimeout = null)
        {
            return await _connection.ExecuteAsync(query, parameters, _transaction, commandTimeout);
        }

        /// <inheritdoc />
        public async Task<T> ExecuteScalarAsync<T>(string query, object? parameters = null, int? commandTimeout = null)
        {
            return await _connection.ExecuteScalarAsync<T>(query, parameters, _transaction, commandTimeout);
        }

        /// <inheritdoc />
        public async Task<int> ExecuteBatchAsync(string query, IEnumerable<object> parameters, int? commandTimeout = null)
        {
            return await _connection.ExecuteAsync(query, parameters, _transaction, commandTimeout);
        }
    }
}