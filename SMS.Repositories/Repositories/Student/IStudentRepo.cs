using SMS.Repositories.Model;

namespace SMS.Repositories.Repositories.Sample;

public interface IStudentRepo
{
    Task<IEnumerable<StudentRepoModel>> GetAllAsync(CancellationToken cancellationToken = default);
}
