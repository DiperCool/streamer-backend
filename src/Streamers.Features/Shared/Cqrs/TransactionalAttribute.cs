using System.Data;

namespace Streamers.Features.Shared.Cqrs;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class TransactionalAttribute(
    IsolationLevel isolationLevel = IsolationLevel.ReadCommitted
) : Attribute
{
    public IsolationLevel IsolationLevel { get; } = isolationLevel;
}
