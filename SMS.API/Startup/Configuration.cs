namespace SMS.API.Startup;

public static class Configuration
{
    public static IConfiguration LoadConfiguration(this WebApplicationBuilder builder, string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#if DEBUG
            .AddJsonFile($"appsettings.{args[0]}.json", optional: true, reloadOnChange: true)
#endif
            .AddEnvironmentVariables()
            .Build();

        builder.WebHost.UseConfiguration(configuration);

        return configuration;
    }
}
