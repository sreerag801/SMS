using SMS.Repositories.Providers;
using SMS.Repositories.Repositories.Sample;
using System.Data;

namespace SMS.Repositories.Persistance;

public class SqlServerUnitOfWork : IUnitOfWork
{
    public SqlServerUnitOfWork(IDbConnection dbConnection)
    {
        Connection = dbConnection;
        StudentRepo = new StudentRepo(this);
    }
    public IDbConnection Connection { get; }

    public IDbTransaction? Transaction  { get; private set; }

    public IStudentRepo StudentRepo { get; }

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
