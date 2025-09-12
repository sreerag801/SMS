using SMS.Repositories.Providers;
using System.Data;

namespace SMS.Repositories.Persistance;

public class SqlServerUnitOfWork(IDbConnection dbConnection) : IUnitOfWork
{
    public IDbConnection Connection { get; } = dbConnection;

    public IDbTransaction? Transaction  { get; private set; }

    public void BeginTransaction()
    {
        if (Transaction != null)
            return;
        Connection.Open();
        Transaction = Connection.BeginTransaction();
    }

    public void Commit()
    {
        Transaction?.Commit();
        Dispose();
    }

    public void Rollback()
    {
        Transaction?.Rollback();
        Dispose();
    }

    public void Dispose()
    {
        Transaction?.Dispose();
        Transaction = null;
        Connection.Close();
    }

    public async Task ExecuteInTransactionAsync(
        Func<Task> operation,
        Func<Task>? finallyAction = null)
    {
        BeginTransaction();
        try
        {
            await operation();

            Commit();
        }
        catch (Exception ex)
        {
            Rollback();
            throw new InvalidOperationException();
        }
        finally
        {
            if (finallyAction != null)
                await finallyAction();
        }
    }
}
