using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMS.Domain.Common;

namespace SMS.Infrastructure.Configurations.Common;
public class BaseEntityConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(at => at.Id);
        builder.Property(at => at.Id).ValueGeneratedOnAdd();

        builder.HasIndex(at => at.EntityId);
        builder.Property(at => at.EntityId).HasDefaultValueSql("NEWID()");
    }
}
