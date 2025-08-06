namespace Shared.Minio;

public interface IBlobStorageConfiguration
{
    string? Uri { get; }
    string? Username { get; }
    string? Password { get; }
}
