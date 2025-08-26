namespace SMS.Service.Shared.Dto;

public record StudentDto
{
    public Guid EntityId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public char Gender { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public IList<EnrollmentDto>? Enrollments { get; set; }
}
