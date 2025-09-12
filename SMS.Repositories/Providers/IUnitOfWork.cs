using System.Data;

namespace SMS.Repositories.Providers;

public interface IUnitOfWork : IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction? Transaction { get; }

    void BeginTransaction();
    void Commit();
    void Rollback();
    Task ExecuteInTransactionAsync(
        Func<Task> operation,
        Func<Task>? finallyAction = null);
}
