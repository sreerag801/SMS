using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities;
using SMS.Infrastructure.Persistance;
using SMS.Service.Common.Interface;
using SMS.Service.Services.Students;

namespace Service.Tests.Tests;

public class StudentServiceTests
{
    private ISMSContext _smsContext = null;
    private IStudentService _studentService = null;
    private Guid _id = Guid.NewGuid();

    public StudentServiceTests()
    {
        var options = new DbContextOptionsBuilder<SMSContext>()
            .UseInMemoryDatabase(databaseName: "sms")
            .Options;
        _smsContext = new SMSContext(options, TimeProvider.System);
        _studentService = new StudentService(_smsContext, timeProvider: TimeProvider.System);

        AddDummyStudents().ConfigureAwait(true);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnStudent()
    {
        var allStudnents = await _studentService.GetByIdAsync(_id);

        Assert.NotNull(allStudnents);
        Assert.Equal("John", allStudnents.FirstName);
    }

    [Fact]
    public async Task GetAllAsync_ReturnDummyStudents()
    {
        var allStudnents = await _studentService.GetAllAsync();

        Assert.NotNull(allStudnents);
        Assert.Single(allStudnents);
    }

    private async Task AddDummyStudents()
    {
        _smsContext.Students.Add(Student.Create(_id, "John", "M", new DateTime(1980, 12, 10), 'M', "john@test.com", "123456789", "test_address"));
        await _smsContext.SaveChangesAsync();
    }
}
