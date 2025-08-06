namespace Shared.Abstractions.Domain;

public interface IHaveAggregateVersion
{
    long OriginalVersion { get; }
}
