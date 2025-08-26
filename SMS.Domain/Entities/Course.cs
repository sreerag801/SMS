using SMS.Domain.Common;

namespace SMS.Domain.Entities;

public class Course : BaseAuditableEntity
{
    private readonly List<Enrollment> _enrollments;
    public string CourseName { get; set; }
    public string? CourseDescription { get; set; }
    public int DurationMonths { get; set; }
    public IReadOnlyCollection<Enrollment>? Enrollments => _enrollments.AsReadOnly();

    public static Course Create(Guid entityId, string courseName, string courseDescription, int durationMonths)
        => new Course(entityId, courseName, courseDescription, durationMonths);

    private Course()
    {
        _enrollments = [];
    }

    private Course(Guid entityId, string courseName, string? courseDescription, int durationMonths)
    {
        EntityId = entityId;
        CourseName = courseName;
        CourseDescription = courseDescription;
        DurationMonths = durationMonths;
        _enrollments = [];
    }
}
