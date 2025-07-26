using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using bgbahasajerman_DataAccessLibrary.Models;

namespace bgbahasajerman_DataAccessLibrary.DataAccess
{
    /// <summary>
    /// Comprehensive examples demonstrating how to use the QueryExecutor class.
    /// These examples show various scenarios and best practices.
    /// </summary>
    public class QueryExecutorUsageExamples
    {
        private readonly QueryExecutor _queryExecutor;

        public QueryExecutorUsageExamples(QueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        #region Basic CRUD Operations

        /// <summary>
        /// Example: Get a single user by ID
        /// </summary>
        public async Task<User?> GetUserExample(int userId)
        {
            const string query = "SELECT * FROM Users WHERE Id = @UserId";
            return await _queryExecutor.QuerySingleAsync<User>(query, new { UserId = userId });
        }

        /// <summary>
        /// Example: Get multiple users with filtering
        /// </summary>
        public async Task<IEnumerable<User>> GetActiveUsersExample()
        {
            const string query = "SELECT * FROM Users WHERE DateRegistered >= @StartDate ORDER BY Username";
            var parameters = new { StartDate = DateTime.Now.AddMonths(-6) };
            return await _queryExecutor.QueryAsync<User>(query, parameters);
        }

        /// <summary>
        /// Example: Create a new user and get the generated ID
        /// </summary>
        public async Task<int> CreateUserExample(string username, string email)
        {
            const string query = "INSERT INTO Users (Username, Email, DateRegistered) VALUES (@Username, @Email, @DateRegistered); SELECT LAST_INSERT_ID();";
            var parameters = new { Username = username, Email = email, DateRegistered = DateTime.UtcNow };
            return await _queryExecutor.ExecuteScalarAsync<int>(query, parameters);
        }

        /// <summary>
        /// Example: Update user information
        /// </summary>
        public async Task<bool> UpdateUserExample(int userId, string newEmail)
        {
            const string query = "UPDATE Users SET Email = @Email WHERE Id = @UserId";
            var parameters = new { Email = newEmail, UserId = userId };
            var rowsAffected = await _queryExecutor.ExecuteAsync(query, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Example: Delete a user
        /// </summary>
        public async Task<bool> DeleteUserExample(int userId)
        {
            const string query = "DELETE FROM Users WHERE Id = @UserId";
            var rowsAffected = await _queryExecutor.ExecuteAsync(query, new { UserId = userId });
            return rowsAffected > 0;
        }

        #endregion

        #region Batch Operations

        /// <summary>
        /// Example: Bulk insert multiple users
        /// </summary>
        public async Task<int> BulkInsertUsersExample(List<User> users)
        {
            const string query = "INSERT INTO Users (Username, Email, DateRegistered) VALUES (@Username, @Email, @DateRegistered)";
            var parameters = users.Select(u => new { u.Username, u.Email, u.DateRegistered });
            return await _queryExecutor.ExecuteBatchAsync(query, parameters);
        }

        #endregion

        #region Scalar Operations

        /// <summary>
        /// Example: Get total user count
        /// </summary>
        public async Task<int> GetUserCountExample()
        {
            const string query = "SELECT COUNT(*) FROM Users";
            return await _queryExecutor.ExecuteScalarAsync<int>(query);
        }

        /// <summary>
        /// Example: Get the latest registration date (with null handling)
        /// </summary>
        public async Task<DateTime?> GetLatestRegistrationDateExample()
        {
            const string query = "SELECT MAX(DateRegistered) FROM Users";
            return await _queryExecutor.ExecuteScalarOrDefaultAsync<DateTime?>(query);
        }

        /// <summary>
        /// Example: Check if a username exists
        /// </summary>
        public async Task<bool> UsernameExistsExample(string username)
        {
            const string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            var count = await _queryExecutor.ExecuteScalarAsync<int>(query, new { Username = username });
            return count > 0;
        }

        #endregion

        #region Transaction Examples

        /// <summary>
        /// Example: Transfer operation between two accounts (demonstrates transaction)
        /// </summary>
        public async Task TransferExample(int fromUserId, int toUserId, decimal amount)
        {
            await _queryExecutor.ExecuteInTransactionAsync(async (executor) =>
            {
                // Debit from source account
                await executor.ExecuteAsync(
                    "UPDATE UserAccounts SET Balance = Balance - @Amount WHERE UserId = @UserId AND Balance >= @Amount",
                    new { Amount = amount, UserId = fromUserId });

                // Credit to destination account
                await executor.ExecuteAsync(
                    "UPDATE UserAccounts SET Balance = Balance + @Amount WHERE UserId = @UserId",
                    new { Amount = amount, UserId = toUserId });

                // Log the transaction
                await executor.ExecuteAsync(
                    "INSERT INTO TransactionLogs (FromUserId, ToUserId, Amount, TransactionDate) VALUES (@FromUserId, @ToUserId, @Amount, @TransactionDate)",
                    new { FromUserId = fromUserId, ToUserId = toUserId, Amount = amount, TransactionDate = DateTime.UtcNow });
            });
        }

        /// <summary>
        /// Example: Transaction with return value
        /// </summary>
        public async Task<int> CreateUserWithProfileExample(string username, string email, string profileData)
        {
            return await _queryExecutor.ExecuteInTransactionAsync(async (executor) =>
            {
                // Create user
                var userId = await executor.ExecuteScalarAsync<int>(
                    "INSERT INTO Users (Username, Email, DateRegistered) VALUES (@Username, @Email, @DateRegistered); SELECT LAST_INSERT_ID();",
                    new { Username = username, Email = email, DateRegistered = DateTime.UtcNow });

                // Create profile
                await executor.ExecuteAsync(
                    "INSERT INTO UserProfiles (UserId, ProfileData) VALUES (@UserId, @ProfileData)",
                    new { UserId = userId, ProfileData = profileData });

                return userId;
            });
        }

        #endregion

        #region Advanced Query Examples

        /// <summary>
        /// Example: Get results as DataTable for dynamic processing
        /// </summary>
        public async Task<DataTable> GetUserReportDataExample()
        {
            const string query = @"
                SELECT 
                    u.Username,
                    u.Email,
                    u.DateRegistered,
                    COUNT(p.Id) as PostCount
                FROM Users u
                LEFT JOIN Posts p ON u.Id = p.UserId
                GROUP BY u.Id, u.Username, u.Email, u.DateRegistered
                ORDER BY PostCount DESC";

            return await _queryExecutor.QueryAsDataTableAsync(query);
        }

        /// <summary>
        /// Example: Complex query with multiple result sets
        /// </summary>
        public async Task<(List<User> Users, int TotalCount)> GetPagedUsersWithCountExample(int pageNumber, int pageSize)
        {
            const string query = @"
                SELECT * FROM Users 
                ORDER BY Username 
                LIMIT @Offset, @PageSize;
                
                SELECT COUNT(*) FROM Users;";

            var offset = (pageNumber - 1) * pageSize;
            var parameters = new { Offset = offset, PageSize = pageSize };

            return await _queryExecutor.QueryMultipleAsync(query, async (gridReader) =>
            {
                var users = (await gridReader.ReadAsync<User>()).ToList();
                var totalCount = await gridReader.ReadSingleAsync<int>();
                return (users, totalCount);
            }, parameters);
        }

        #endregion

        #region Utility Examples

        /// <summary>
        /// Example: Custom operation with direct connection access
        /// </summary>
        public async Task<string> GetDatabaseVersionExample()
        {
            return await _queryExecutor.ExecuteWithConnectionAsync(async (connection) =>
            {
                // Use Dapper for consistency instead of raw IDbCommand
                return await connection.QuerySingleAsync<string>("SELECT VERSION()");
            });
        }

        /// <summary>
        /// Example: Health check
        /// </summary>
        public async Task<bool> CheckDatabaseHealthExample()
        {
            return await _queryExecutor.TestConnectionAsync();
        }

        #endregion

        #region Error Handling Examples

        /// <summary>
        /// Example: Proper error handling
        /// </summary>
        public async Task<User?> GetUserWithErrorHandlingExample(int userId)
        {
            try
            {
                const string query = "SELECT * FROM Users WHERE Id = @UserId";
                return await _queryExecutor.QuerySingleAsync<User>(query, new { UserId = userId });
            }
            catch (Exception ex)
            {
                // Log the error (use your preferred logging framework)
                Console.WriteLine($"Error retrieving user {userId}: {ex.Message}");
                
                // Decide how to handle the error:
                // - Return null/default value
                // - Rethrow the exception
                // - Throw a custom exception
                // - Return a fallback value
                
                return null; // In this example, we return null
            }
        }

        /// <summary>
        /// Example: Transaction with proper error handling
        /// </summary>
        public async Task<bool> SafeTransactionExample(int userId, string newData)
        {
            try
            {
                await _queryExecutor.ExecuteInTransactionAsync(async (executor) =>
                {
                    await executor.ExecuteAsync(
                        "UPDATE Users SET SomeField = @NewData WHERE Id = @UserId",
                        new { NewData = newData, UserId = userId });

                    await executor.ExecuteAsync(
                        "INSERT INTO ChangeLog (UserId, ChangeType, ChangeDate) VALUES (@UserId, @ChangeType, @ChangeDate)",
                        new { UserId = userId, ChangeType = "DataUpdate", ChangeDate = DateTime.UtcNow });
                });

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Transaction failed for user {userId}: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}