using Service.Tests.Fixture;
using SMS.Service.Services.Students;

namespace Service.Tests.Tests;

public class StudentServiceTests : IClassFixture<SMSContextFixture>
{
    private IStudentService _studentService;
    private readonly SMSContextFixture _fixture;

    public StudentServiceTests(SMSContextFixture fixture)
    {
        _fixture = fixture;
        _studentService = new StudentService(_fixture!.SMSContext, timeProvider: TimeProvider.System);
    }

    [Fact]
    public async Task GetAllAsync_ReturnDummyStudents()
    {
        var allStudents = await _studentService.GetAllAsync();

        Assert.NotNull(allStudents);
        Assert.True(allStudents.Count() > 0);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnStudent()
    {
        var firstStudent = (await _studentService.GetAllAsync()).First();
        var student = await _studentService.GetByIdAsync(firstStudent.EntityId);
        
        Assert.NotNull(student);
        Assert.Equal("John", student.FirstName);
    }
}
