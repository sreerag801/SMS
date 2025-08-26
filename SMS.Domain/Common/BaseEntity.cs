namespace SMS.Domain.Common;
public class BaseEntity
{
    public long Id { get; set; }
    public Guid EntityId { get; init; }
}
