using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SMS.Infrastructure.Persistance;
using System.Net.Http.Json;
using System.Text;
using Testcontainers.MsSql;

namespace SMS.WebApi.IntegrationTests.Fixtures;

public class WebAppEFFixture : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private WebApplicationFactory<Program> _delegatedFactory = null!;
    private IServiceScopeFactory _scopeFactory = null!;
    private IServiceScope _scopeService = null!;
    private SMSContext? _context;
    private string _connectionString;
    private Fixture _fixture = new();

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

    public Task<HttpResponseMessage> HttpSendAsyncWithBasicAuth(
        HttpMethod httpMethod,
        string endpoint,
        object? content = null,
        (string, string)[]? additionalHeaders = null
    ) => HttpSendAsyncWithBasicAuthInternal(httpMethod, endpoint, content, additionalHeaders);


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
            _connectionString = _sqlServer.GetConnectionString() + ";Database=Sms;";
            var options = new DbContextOptionsBuilder<SMSContext>().UseSqlServer(_connectionString).Options;

            _context = new SMSContext(options, TimeProvider.System);
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
            await _context.Database.MigrateAsync();

            _factory = new CustomWebApplicationFactory(new SqlConnection(_connectionString));
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
            _scopeService = _scopeFactory.CreateScope();

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
        // Seed Course
        await SeedSources();
    }

    private async Task SeedSources()
    {
        using var transaction = await Context.Database.BeginTransactionAsync();
        await Context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Courses] ON");
        var courses = _fixture.Create<Domain.Entities.Course>();
        
        Context.Courses.Add(courses);

        await Context.SaveChangesAsync();
        await Context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Courses] OFF");
        await transaction.CommitAsync();
    }

    private async Task<HttpResponseMessage> HttpSendAsyncWithBasicAuthInternal(
        HttpMethod method,
        string endpoint,
        object? content = null,
        (string, string)[]? additionalHeaders = null
    )
    {
        var factory = _delegatedFactory ?? _factory;
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var message = FillCommonHeaders(method, endpoint, content, additionalHeaders);
        var username = "username";
        var password = "password";

        var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
        message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "TestScheme",
            Convert.ToBase64String(byteArray)
        );

        return await client.SendAsync(message);
    }

    private static HttpRequestMessage FillCommonHeaders(
        HttpMethod method,
        string endpoint,
        object? content = null,
        (string, string)[]? additionalHeaders = null
    )
    {
        var message = new HttpRequestMessage(method, endpoint);

        if (content != null)
        {
            if (content is HttpContent httpContent)
            {
                message.Content = httpContent;
            }
            else
            {
                message.Content = JsonContent.Create(content);
            }
        }

        if (additionalHeaders?.Length > 0)
            foreach (var (key, value) in additionalHeaders)
                message.Headers.Add(key, value);

        return message;
    }

    public void ReconfigureTestServices(Action<IServiceCollection> services)
    {
        _delegatedFactory = ((CustomWebApplicationFactory)_factory).ReconfigureTestServices(services);
        _scopeFactory = _delegatedFactory.Services.GetRequiredService<IServiceScopeFactory>();
        _scopeService = _scopeFactory.CreateScope();
    }
}
