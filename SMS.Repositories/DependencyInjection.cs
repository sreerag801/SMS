using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMS.Repositories.Persistance;
using SMS.Repositories.Providers;
using SMS.Repositories.Repositories.Sample;
using System.Data;

namespace SMS.Repositories;
public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment = false
    )
    {
        services.AddTransient<IDbConnection>(_ => new SqlConnection(configuration.GetConnectionString("Sms")));
        services.AddTransient<IUnitOfWork, SqlServerUnitOfWork>();
        services.AddTransient<IStudentRepo, StudentRepo>();
        return services;
    }
}
