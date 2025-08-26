using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMS.Domain.Common;

namespace SMS.Infrastructure.Configurations.Common;
public class BaseAuditableEntityConfiguration<T> : BaseEntityConfiguration<T> where T : BaseAuditableEntity
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        string tableName = typeof(T).Name.Pluralize();

        base.Configure(builder);

        builder.HasQueryFilter(p => p.DeletedAt == null);

        builder.ToTable(
            tableName,
            c =>
                c.IsTemporal(c =>
                {
                    c.HasPeriodStart("SysStartTime");
                    c.HasPeriodEnd("SysEndTime");
                    c.UseHistoryTable($"{tableName}History");
                })
        );
    }
}
