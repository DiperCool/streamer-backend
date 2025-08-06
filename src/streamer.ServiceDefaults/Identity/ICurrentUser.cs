namespace streamer.ServiceDefaults.Identity;

public interface ICurrentUser
{
    public bool IsAuthenticated { get; }
    public string UserId { get; }
}
