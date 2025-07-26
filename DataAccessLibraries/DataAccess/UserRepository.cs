using System.Collections.Generic;
using System.Threading.Tasks;
using bgbahasajerman_DataAccessLibrary.Models;

namespace bgbahasajerman_DataAccessLibrary.DataAccess
{
    /// <summary>
    /// Repository for managing user data.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly QueryExecutor _queryExecutor;

        /// <summary>
        /// Initializes a new instance of the UserRepository class.
        /// </summary>
        /// <param name="queryExecutor">The query executor to use for database operations.</param>
        public UserRepository(QueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            const string query = "SELECT * FROM Users;";
            return await _queryExecutor.QueryAsync<User>(query);
        }

        /// <inheritdoc />
        public async Task<User> GetUserByIdAsync(int id)
        {
            const string query = "SELECT * FROM Users WHERE Id = @Id;";
            return await _queryExecutor.QuerySingleAsync<User>(query, new { Id = id });
        }

        /// <inheritdoc />
        public async Task<int> CreateUserAsync(User user)
        {
            // MySQL uses LAST_INSERT_ID() instead of SCOPE_IDENTITY()
            const string query = "INSERT INTO Users (Username, Email) VALUES (@Username, @Email); SELECT LAST_INSERT_ID();";
            return await _queryExecutor.ExecuteScalarAsync<int>(query, user);
        }

        /// <inheritdoc />
        public async Task UpdateUserAsync(User user)
        {
            const string query = "UPDATE Users SET Username = @Username, Email = @Email WHERE Id = @Id;";
            await _queryExecutor.ExecuteAsync(query, user);
        }

        /// <inheritdoc />
        public async Task DeleteUserAsync(int id)
        {
            const string query = "DELETE FROM Users WHERE Id = @Id;";
            await _queryExecutor.ExecuteAsync(query, new { Id = id });
        }
    }
}
