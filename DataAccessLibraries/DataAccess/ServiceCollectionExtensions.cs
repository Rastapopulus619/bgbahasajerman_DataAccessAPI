using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace bgbahasajerman_DataAccessLibrary.DataAccess
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> to easily register
    /// the data access layer services with a Dependency Injection container.
    /// This is the primary entry point for the host application to configure this library.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all necessary data access services to the specified <see cref="IServiceCollection"/>.
        /// This method should be called from the host application's startup (e.g., Program.cs).
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configuration">The application's configuration, used to retrieve connection strings and other settings.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the IDbConnectionFactory as a Singleton.
            // A Singleton instance means that only one instance of the factory will be created
            // and reused throughout the application's lifetime. This is appropriate because
            // the factory itself is stateless and efficient to reuse.
            services.AddSingleton<IDbConnectionFactory>(sp =>
            {
                // Retrieve the connection string from the application's configuration.
                // The host application must provide a "MySqlConnection" string in its appsettings.json or similar.
                var connectionString = configuration.GetConnectionString("MySqlConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'MySqlConnection' not found in configuration. Please ensure it is defined in appsettings.json or similar configuration source.");
                }
                // Here, we explicitly create a MySqlConnectionFactory. If you wanted to support
                // multiple database types, you would add logic here to choose the correct factory
                // based on a configuration setting (e.g., "DatabaseProvider": "MySQL" or "SQLServer").
                return new MySqlConnectionFactory(configuration);
            });

            // Register the QueryExecutor as a Scoped service.
            // A Scoped service means that a new instance of QueryExecutor will be created once per client request
            // (e.g., per HTTP request in a web application). This ensures that any state within the QueryExecutor
            // (though it's currently stateless) is isolated to a single request.
            services.AddScoped<QueryExecutor>();

            // Register the IUserRepository and its concrete implementation UserRepository as Scoped services.
            // Like QueryExecutor, a new instance of UserRepository will be created per client request.
            // This allows repositories to safely depend on other Scoped services (like QueryExecutor)
            // and ensures proper resource management within the scope of a single request.
            services.AddScoped<IUserRepository, UserRepository>();

            // You would register other repository interfaces and their implementations here as needed.
            // Example: services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
