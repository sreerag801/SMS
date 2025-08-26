using SMS.Domain.Common;

namespace SMS.Domain.Entities;

public class Student: BaseAuditableEntity
{
    private readonly List<Enrollment> _enrollments;
    public string? FirstName { get; set; } 
    public string? LastName { get; set; } 
    public DateTime DateOfBirth { get; set; }
    public char Gender { get; set; } 
    public string? Email { get; set; } 
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public IReadOnlyCollection<Enrollment>? Enrollments => _enrollments.AsReadOnly();

    private Student()
    {
        _enrollments = [];
    }

    private Student(Guid entityId, string firstName, string lastName, DateTime dateOfBirth, char gender, string email, string phoneNumber, string address)
    {
        Address = address;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Email = email;
        PhoneNumber = phoneNumber;
        CreatedAt = DateTime.UtcNow;
        EntityId = entityId;
        _enrollments = [];
    }

    public static Student Create(Guid entityId, string firstName, string lastName, DateTime dateOfBirth, char gender, string email, string phoneNumber, string address)
        => new Student(entityId, firstName, lastName, dateOfBirth, gender, email, phoneNumber, address);

    public void AddEnrollment(Guid entityId, bool isActive, DateTime enrollmentDate, long studentId, long courseId)
    {
        _enrollments.Add(Enrollment.Create(entityId, isActive, enrollmentDate, studentId, courseId));
    }

    public void UpdateEnrollment(Guid studentEntity, long studentId, bool isActive, DateTime enrollmentDate, long courseId)
    {
        var existingEnrollment = _enrollments.FirstOrDefault(e => e.StudentId == studentId);
        if (existingEnrollment == null)
            throw new Exception($"No enrollment available for the student {studentEntity}");

        existingEnrollment.IsActive = isActive;
        existingEnrollment.EnrollmentDate = enrollmentDate;
        existingEnrollment.StudentId = studentId;
        existingEnrollment.CourseId = courseId;
    }
}
