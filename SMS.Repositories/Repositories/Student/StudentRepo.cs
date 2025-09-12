using SMS.Infrastructure.Persistance;
using SMS.Repositories.Model;
using SMS.Repositories.Providers;

namespace SMS.Repositories.Repositories.Sample;

public class StudentRepo(IUnitOfWork unitOfWork) : RepositoryBase(unitOfWork), IStudentRepo 
{
    public async Task<IEnumerable<StudentRepoModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await QueryAsync<StudentRepoModel>(
            """
            SELECT * FROM dbo.Students
            """,
            null, cancellationToken);
    }
}
