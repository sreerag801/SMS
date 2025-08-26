using SMS.Service.Shared.Dto;

namespace SMS.Service.Services.Course;
public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync(CancellationToken cancellationToken = default);
    Task<CourseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CourseDto course, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<CourseDto> UpdateAsync(Guid courseId, CourseDto request, CancellationToken cancellationToken);
}
