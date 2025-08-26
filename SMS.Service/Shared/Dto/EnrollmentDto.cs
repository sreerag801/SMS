namespace SMS.Service.Shared.Dto;

public record EnrollmentDto
{
    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; }
    public CourseDto Course { get; set; }
}
