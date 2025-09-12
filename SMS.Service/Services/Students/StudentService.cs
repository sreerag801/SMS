using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities;
using SMS.Repositories.Repositories.Sample;
using SMS.Service.Common.Exceptions;
using SMS.Service.Common.Interface;
using SMS.Service.Shared.Dto;
using SMS.Service.Shared.Requests;

namespace SMS.Service.Services.Students;

public class StudentService(ISMSContext context, IStudentRepo studentRepo, TimeProvider timeProvider) : IStudentService
{
    public async Task<Guid> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var course = await context.Courses.FindAsync(request.CourseId);
        if (course == null)
            throw new Exception($"Course with Id {request.CourseId} not found.");

        var student = Student
            .Create(Guid.NewGuid(), request.FirstName, request.LastName, request.DateOfBirth, request.Gender, request.Email, request.PhoneNumber, request.Address);
        student.AddEnrollment(Guid.NewGuid(), true, timeProvider.GetUtcNow().UtcDateTime, student.Id, request.CourseId.Value);
        
        context.Students.Add(student);
        await context.SaveChangesAsync();

        return student.EntityId;
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingStudent = context
            .Students
            .Include(st => st.Enrollments)
            .Where(st => st.EntityId == id).FirstOrDefault();

        if (existingStudent == null)
            throw new Exception("There is no student with id {id} to delete.");

        context.Students.Remove(existingStudent);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var studentDetails = await context
            .Students
            .AsSplitQuery()
            .Include(x => x.Enrollments!)
                .ThenInclude(x => x.Course)
            .ToListAsync(cancellationToken);

        return studentDetails.Select(s =>
            new StudentDto
            {
                EntityId= s.EntityId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Address = s.Address,
                DateOfBirth = s.DateOfBirth,
                Email = s.Email,
                Enrollments = s.Enrollments?.Select(e =>
                    new EnrollmentDto
                    {
                        EnrollmentDate = e.EnrollmentDate,
                        IsActive = e.IsActive,
                        Course = new CourseDto
                        {
                            CourseId = e.CourseId,
                            CourseDescription = e?.Course?.CourseDescription,
                            CourseName = e?.Course?.CourseName,
                            DurationMonths = e?.Course?.DurationMonths ?? 0
                        }
                    }).ToList(),
                Gender = s.Gender,
                PhoneNumber = s.PhoneNumber,
            });
    }

    public async Task<StudentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var studentDetails = await context
            .Students
            .AsSplitQuery()
            .Where(s => s.EntityId == id)
            .Include(x => x.Enrollments!)
                .ThenInclude(x => x.Course)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFountException($"No student available for the id {id}");

        return new StudentDto
        {
            EntityId = studentDetails.EntityId,
            FirstName = studentDetails.FirstName,
            LastName = studentDetails.LastName,
            Address = studentDetails.Address,
            DateOfBirth = studentDetails.DateOfBirth,
            Email = studentDetails.Email,
            Enrollments = studentDetails.Enrollments?.Select(e =>
            new EnrollmentDto
            {
                EnrollmentDate = e.EnrollmentDate,
                IsActive = e.IsActive,
                Course = new CourseDto
                {
                    CourseId = e.CourseId,
                    CourseDescription = e?.Course?.CourseDescription,
                    CourseName = e?.Course?.CourseName,
                    DurationMonths = e?.Course?.DurationMonths ?? 0
                }
            }).ToList(),
            Gender = studentDetails.Gender,
            PhoneNumber = studentDetails.PhoneNumber,
        };
    }

    public async Task<Guid> UpdateAsync(Guid id, CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var course = await context.Courses.FindAsync(request.CourseId);
        if (course == null)
            throw new Exception($"Course with Id {request.CourseId} not found.");

        var existingStudent = context
            .Students
            .Include(st => st.Enrollments)
            .Where(st => st.EntityId == id).FirstOrDefault();

        if (existingStudent == null)
            throw new NotFountException($"There is not student available for the id {id}.");
        existingStudent.DateOfBirth = request.DateOfBirth;
        existingStudent.Gender = request.Gender;
        existingStudent.PhoneNumber = request.PhoneNumber;
        existingStudent.FirstName = request.FirstName;
        existingStudent.LastName = request.LastName;
        existingStudent.Email = request.Email;
        existingStudent.UpdateEnrollment(existingStudent.EntityId, existingStudent.Id, true, timeProvider.GetUtcNow().UtcDateTime, course.Id);

        context.Students.Update(existingStudent);

        await context.SaveChangesAsync();

        return existingStudent.EntityId;
    }

    public async Task<IEnumerable<StudentDto>> GetStudentsFromDapperRepoASync(CancellationToken cancellationToken)
    {
        return (await studentRepo.GetAllAsync(cancellationToken)).Select(s =>
            new StudentDto
            {
                FirstName = s.FirstName,
                LastName = s.LastName,
                Address = s.Address,
                DateOfBirth = s.DateOfBirth,
                Email = s.Email,
                Gender = s.Gender,
                PhoneNumber = s.PhoneNumber,
            });
    }
}
