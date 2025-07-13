namespace Common;

public abstract class AuditableEntity : Entity
{
    public DateTimeOffset InsertionDate { get; private init; }
    public DateTimeOffset LastModifiedDate { get; private set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(Guid id)
        : base(id)
    {
        InsertionDate = DateTimeOffset.UtcNow;
        LastModifiedDate = DateTimeOffset.UtcNow;
    }

    public void ChangeLastModifiedDate()
    {
        LastModifiedDate = DateTimeOffset.UtcNow;
    }
}