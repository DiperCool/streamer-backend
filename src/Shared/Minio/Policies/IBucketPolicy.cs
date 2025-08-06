namespace Shared.Minio.Policies;

public interface IBucketPolicy
{
    string Get(string bucket);
}
