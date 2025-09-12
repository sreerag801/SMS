using SMS.Service.Shared.Dto;
using SMS.Service.Shared.Requests;

namespace SMS.Service.Services.Students;

public interface IStudentService
{
    // EF Core
    Task<IEnumerable<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StudentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(Guid id, CreateStudentRequest request, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);


    // Dapper
    Task<IEnumerable<StudentDto>> GetStudentsFromDapperRepoASync(CancellationToken cancellationToken);
}