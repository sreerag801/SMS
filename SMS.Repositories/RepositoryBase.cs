using Dapper;
using Microsoft.Extensions.DependencyInjection;
using SMS.Repositories.Providers;
using System.Data;

namespace SMS.Infrastructure.Persistance;

public class RepositoryBase(IUnitOfWork unitOfWork, IServiceProvider? serviceProvider = null)
{
    private const int CommandTimeout = 300; 

    protected async Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object? parameters,
            CancellationToken ct
        )
    {
        var cmd = new CommandDefinition(
            sql,
            parameters,
            unitOfWork.Transaction,
            CommandTimeout,
            cancellationToken: ct
        );
        return await unitOfWork.Connection.QueryAsync<T>(cmd) ?? [];
    }

    protected async Task<IEnumerable<T>> QueryWithNewConnectionAsync<T>(
            string sql,
            object? parameters,
            CancellationToken ct
        )
    {
        _ = serviceProvider ?? throw new NullReferenceException(nameof(serviceProvider));
        using var connection = serviceProvider.GetRequiredService<IDbConnection>();
        var cmd = new CommandDefinition(
            sql,
            parameters,
            unitOfWork.Transaction,
            cancellationToken: ct
        );
        return await connection.QueryAsync<T>(cmd) ?? [];
    }

    protected async Task<T?> QueryFirstOrDefaultAsync<T>(
        string sql,
        object? parameters,
        CancellationToken ct
    )
    {
        var cmd = new CommandDefinition(
            sql,
            parameters,
            unitOfWork.Transaction,
            cancellationToken: ct
        );
        return await unitOfWork.Connection.QueryFirstOrDefaultAsync<T?>(cmd);
    }

    protected async Task ExecuteAsync(string sql, object? parameters, CancellationToken ct)
    {
        var cmd = new CommandDefinition(
            sql,
            parameters,
            unitOfWork.Transaction,
            cancellationToken: ct
        );
        await unitOfWork.Connection.ExecuteAsync(cmd);
    }

    protected async Task<T?> ExecuteScalarAsync<T>(
        string sql,
        object? parameters,
        CancellationToken ct
    )
    {
        var cmd = new CommandDefinition(
            sql,
            parameters,
            unitOfWork.Transaction,
            cancellationToken: ct
        );
        return await unitOfWork.Connection.ExecuteScalarAsync<T>(cmd);
    }
}
