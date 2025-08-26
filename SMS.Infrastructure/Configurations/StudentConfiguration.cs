using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMS.Domain.Entities;
using SMS.Infrastructure.Configurations.Common;

namespace SMS.Infrastructure.Configurations;
public class StudentConfiguration : BaseAuditableEntityConfiguration<Student>, IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        base.Configure(builder);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName)
            .HasMaxLength(50);

        builder.Property(e => e.LastName)
            .HasMaxLength(50);

        builder.Property(e => e.Email)
            .HasMaxLength(100);

        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");

        builder.Property(e => e.Gender)
            .HasColumnType("char(1)");

        builder.Property(e => e.Address)
            .HasMaxLength(250);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(15);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

        builder
            .HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
