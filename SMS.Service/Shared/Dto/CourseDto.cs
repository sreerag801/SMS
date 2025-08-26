namespace SMS.Service.Shared.Dto;

public record CourseDto
{
    public long CourseId { get; set; }
    public required string CourseName { get; set; }
    public string? CourseDescription { get; set; }
    public int DurationMonths { get; set; }
}