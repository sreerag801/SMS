using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SMS.Domain.Entities;

namespace SMS.Service.Common.Interface;
public interface ISMSContext
{
    public DbSet<Student> Students { get; }
    public DbSet<Enrollment> Enrollments { get; }
    public DbSet<Course> Courses { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    IDbContextTransaction BeginTransaction();
}
