using DbUp;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SMS.Infrastructure.Persistance;
using System.Reflection;
using Testcontainers.MsSql;

namespace SMS.WebApi.IntegrationTests.Fixtures;

public class WebAppSqlFixture : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private WebApplicationFactory<Program> _delegatedFactory = null!;
    private IServiceScopeFactory _scopeFactory = null!;
    private IServiceScope _scopeService = null!;
    private SMSContext? _context;
    private string _connectionString;

    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2019-CU26-ubuntu-20.04")
        .WithEnvironment("MSSQL_AGENT_ENABLED", "True")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithAutoRemove(true)
        .Build();

    public SMSContext Context => _context ?? throw new Exception("EF Context is null.");
    public HttpClient GetClient => _factory.CreateClient();
    public IServiceScope Scope => _scopeService;

    public Task DisposeAsync()
    {
        return _sqlServer.DisposeAsync().AsTask();
    }

    public async Task InitializeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await _sqlServer.StartAsync();
            _connectionString = _sqlServer.GetConnectionString();
            var options = new DbContextOptionsBuilder<SMSContext>().UseSqlServer(_connectionString).Options;

            _context = new SMSContext(options, TimeProvider.System);
            await _context.Database.MigrateAsync();

            _context = new SMSContext(options, TimeProvider.System);
            _factory = new CustomWebApplicationFactory(new SqlConnection(_connectionString));

            await SeedTestDataAsync();

        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task SeedTestDataAsync()
    {
        var upgrader = DeployChanges
            .To.SqlDatabase(Context.Database.GetConnectionString())
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var report = upgrader.PerformUpgrade();

        if (!report.Successful)
        {
            throw new Exception(
                $"Script seeding failed. Please check Script file {report.ErrorScript.Name}. Error: {report.Error}"
            );
        }

        await Task.CompletedTask;
    }

    public void ReconfigureTestServices(Action<IServiceCollection> services)
    {
        _delegatedFactory = ((CustomWebApplicationFactory)_factory).ReconfigureTestServices(services);
        _scopeFactory = _delegatedFactory.Services.GetRequiredService<IServiceScopeFactory>();
        _scopeService = _scopeFactory.CreateScope();
    }
}
