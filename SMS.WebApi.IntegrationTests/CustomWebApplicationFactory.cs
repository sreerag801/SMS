using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SMS.Infrastructure.Interceptors;
using SMS.Infrastructure.Persistance;
using System.Data.Common;

namespace SMS.WebApi.IntegrationTests;
public class CustomWebApplicationFactory(DbConnection connection, Action<IServiceCollection>? overrideRegistrations = null)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder
            .ConfigureAppConfiguration(
                (context, config) =>
                {
                    config.AddJsonFile("appsettings.test.json", optional: true, reloadOnChange: true);
                }
            )
            .ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<SMSContext>>();
                services.RemoveAll<IHostedService>();
                services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();
                services.AddDbContext<SMSContext>((sp, options) =>
                {
                    options.UseSqlServer(connection);
                    options.AddInterceptors(sp.GetRequiredService<ISaveChangesInterceptor>());
                });

                overrideRegistrations?.Invoke(services);
            });
    }

    public WebApplicationFactory<Program> ReconfigureTestServices(Action<IServiceCollection> services) =>
        WithWebHostBuilder(builder => builder.ConfigureTestServices(services));
}
