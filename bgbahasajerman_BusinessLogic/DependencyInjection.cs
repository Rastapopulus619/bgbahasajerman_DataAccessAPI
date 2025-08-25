using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using bgbahasajerman_DataAccessLibrary.DataAccess;
using bgbahasajerman_DataAccessLibrary.Repositories.Students;
using bgbahasajerman_BusinessLogic.Interfaces;
using bgbahasajerman_BusinessLogic.Services;

namespace bgbahasajerman_BusinessLogic
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the DB connection factory implementation (MySqlConnectionFactory depends on IConfiguration)
            services.AddScoped<IDbConnectionFactory, MySqlConnectionFactory>();

            // Register QueryExecutor which depends on IDbConnectionFactory
            services.AddScoped<QueryExecutor>();

            // register repository and service
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentService, StudentService>();

            return services;
        }
    }
}
