namespace Shared.Abstractions.Domain;
public abstract class Entity : Entity<Guid>;

public abstract class Entity<T> : IHaveAggregateVersion
{
    public T Id { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    private const long NewAggregateVersion = 0;
    public long OriginalVersion { get; private set; } = NewAggregateVersion;
    public List<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
