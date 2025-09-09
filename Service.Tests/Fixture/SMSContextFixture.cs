using Microsoft.EntityFrameworkCore;
using SMS.Domain.Entities;
using SMS.Infrastructure.Persistance;

namespace Service.Tests.Fixture;
public class SMSContextFixture : IAsyncLifetime
{
    public SMSContext? SMSContext { get; private set; } = null!;

    public Task DisposeAsync()
    {
        if (SMSContext != null)
            SMSContext.Dispose();

        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<SMSContext>()
           .UseInMemoryDatabase(databaseName: "sms")
           .Options;
        SMSContext = new SMSContext(options, TimeProvider.System);
        AddDummyStudents().ConfigureAwait(true);
        return Task.CompletedTask;
    }

    private async Task AddDummyStudents()
    {
        SMSContext!.Students.Add(Student.Create(Guid.NewGuid(), "John", "M", new DateTime(1980, 12, 10), 'M', "john@test.com", "123456789", "test_address"));
        await SMSContext!.SaveChangesAsync();
    }
}
