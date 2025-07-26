using System.Collections.Generic;
using System.Threading.Tasks;
using bgbahasajerman_DataAccessLibrary.Models;

namespace bgbahasajerman_DataAccessLibrary.DataAccess
{
    /// <summary>
    /// Defines the contract for a user repository.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets all users from the database.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user with the specified ID, or null if not found.</returns>
        Task<User> GetUserByIdAsync(int id);

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>The ID of the newly created user.</returns>
        Task<int> CreateUserAsync(User user);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user to update.</param>
        Task UpdateUserAsync(User user);

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        Task DeleteUserAsync(int id);
    }
}
