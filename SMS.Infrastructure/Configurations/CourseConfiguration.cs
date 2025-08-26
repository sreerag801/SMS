using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMS.Domain.Entities;
using SMS.Infrastructure.Configurations.Common;

namespace SMS.Infrastructure.Configurations;
public class CourseConfiguration : BaseAuditableEntityConfiguration<Course>, IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        base.Configure(builder);

        // Table name
        builder.ToTable("Courses");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.CourseName)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(c => c.CourseDescription)
               .HasMaxLength(1000);


        // Audit fields
        builder.Property(c => c.CreatedAt)
               .IsRequired();

        // Relationships
        builder.HasMany(c => c.Enrollments)
               .WithOne(e => e.Course)
               .HasForeignKey(e => e.CourseId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
