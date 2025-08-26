using SMS.Domain.Common;

namespace SMS.Domain.Entities;
public class Enrollment : BaseEntity
{
    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; }
    
    public long StudentId { get; set; }
    public Student? Student { get; set; }

    public long CourseId { get; set; }
    public Course? Course { get; set; }

    public static Enrollment Create(Guid entityId, bool isActive, DateTime enrollmentDate, long studentId, long courseId)
        => new Enrollment
        {
            EnrollmentDate = enrollmentDate,
            EntityId = entityId,
            StudentId = studentId,
            CourseId = courseId,
            IsActive = isActive,
        };
}
