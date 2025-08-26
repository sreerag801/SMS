using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMS.Infrastructure.Interceptors;
using SMS.Infrastructure.Persistance;
using SMS.Service.Common.Interface;

namespace SMS.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    IConfiguration configuration,
    bool isDevelopment = false
    )
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();
        services.AddDbContext<ISMSContext, SMSContext>(
            (sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Sms"));
                options.AddInterceptors(sp.GetRequiredService<ISaveChangesInterceptor>());
                if (isDevelopment)
                    options.EnableSensitiveDataLogging();
            }
        );
        return services;
    }
}
