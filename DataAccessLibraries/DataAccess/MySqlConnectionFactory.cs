using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace bgbahasajerman_DataAccessLibrary.DataAccess
{
    /// <summary>
    /// A concrete implementation of the IDbConnectionFactory for creating MySQL database connections.
    /// This class knows the specific details about connecting to a MySQL database.
    /// </summary>
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string? _connectionString;

        /// <summary>
        /// Initializes a new instance of the MySqlConnectionFactory.
        /// It depends on IConfiguration to retrieve the necessary connection string.
        /// This dependency will be provided by the host application's DI container.
        /// </summary>
        /// <param name="configuration">The application's configuration, which holds the connection strings.</param>
        public MySqlConnectionFactory(IConfiguration configuration)
        {
            // Retrieves the connection string named "MySqlConnection" from the host application's
            // configuration file (e.g., appsettings.json). This keeps credentials out of the code.
            _connectionString = configuration.GetConnectionString("MySqlConnection");
        }

        /// <summary>
        /// Creates a new instance of MySqlConnection.
        /// This method is called by the QueryExecutor (or repositories) whenever a database operation is needed.
        /// ADO.NET connection pooling ensures that creating new connection objects like this is efficient.
        /// </summary>
        /// <returns>A new, unopened MySqlConnection object.</returns>
        public IDbConnection CreateConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Connection string 'MySqlConnection' is not configured.");
            }

            return new MySqlConnection(_connectionString);
        }
    }
}
