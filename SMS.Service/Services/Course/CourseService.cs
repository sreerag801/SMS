using Microsoft.EntityFrameworkCore;
using SMS.Service.Common.Exceptions;
using SMS.Service.Common.Interface;
using SMS.Service.Shared.Dto;

namespace SMS.Service.Services.Course;
internal class CourseService(ISMSContext context) : ICourseService
{
    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Courses.Select(c => new CourseDto
        {
            CourseDescription = c.CourseDescription,
            CourseName = c.CourseName,
            DurationMonths = c.DurationMonths,
            CourseId = c.Id
        }).ToListAsync(cancellationToken);
    }

    public async Task<CourseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Courses
            .Where(c => c.EntityId == id)
            .Select(c => new CourseDto
            {
                CourseDescription = c.CourseDescription,
                CourseName = c.CourseName,
                DurationMonths = c.DurationMonths,
                CourseId = c.Id
            }).FirstOrDefaultAsync(cancellationToken) ?? throw new NotFountException($"There is no course available for the given id {id}.");
    }

    public async Task<Guid> CreateAsync(CourseDto course, CancellationToken cancellationToken = default)
    {
        Guid courceId = Guid.NewGuid();
        context.Courses.Add(Domain.Entities.Course.Create(courceId, course.CourseName, course.CourseDescription ?? string.Empty, course.DurationMonths));
        await context.SaveChangesAsync(cancellationToken);

        return courceId;
    }

    public async Task DeleteAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var course = await context.Courses.Where(c => c.EntityId == courseId).FirstOrDefaultAsync(cancellationToken);

        if (course == null)
            throw new Exception($"There is no course available for the Id {courseId} to delete.");

        context.Courses.Remove(course);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<CourseDto> UpdateAsync(Guid courseId, CourseDto request, CancellationToken cancellationToken)
    {
        var course = await context.Courses.Where(c => c.EntityId == courseId).FirstOrDefaultAsync(cancellationToken);

        if (course == null)
            throw new Exception($"There is no course available for the Id {courseId} to delete.");

        course.CourseName = request.CourseName;
        course.CourseDescription = request.CourseDescription;
        course.DurationMonths = request.DurationMonths;

        context.Courses.Remove(course);
        await context.SaveChangesAsync(cancellationToken);

        return request;
    }
}
