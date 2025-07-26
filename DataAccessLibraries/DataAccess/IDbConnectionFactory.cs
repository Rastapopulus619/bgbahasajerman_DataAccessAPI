
using System.Data;

namespace bgbahasajerman_DataAccessLibrary.DataAccess
{
    /// <summary>
    /// Defines the contract for a factory that creates database connections.
    /// This interface is the cornerstone of the abstraction, allowing the rest of the library
    /// to request a database connection without needing to know the specific database technology (e.g., MySQL, SQL Server).
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Creates and returns a new database connection instance.
        /// The calling code is responsible for managing the connection's lifecycle (opening, closing, disposing),
        /// typically within a 'using' block to ensure resources are properly released.
        /// </summary>
        /// <returns>An initialized IDbConnection object, ready to be opened.</returns>
        IDbConnection CreateConnection();
    }
}
