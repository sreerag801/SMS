using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SMS.Domain.Entities;
using SMS.Service.Common.Interface;
using System.Reflection;

namespace SMS.Infrastructure.Persistance;
public class SMSContext : DbContext, ISMSContext
{
    public SMSContext(): base()
    {
    }

    public SMSContext(DbContextOptions<SMSContext> options, TimeProvider _)
        : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public IDbContextTransaction BeginTransaction() => Database.BeginTransaction();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
