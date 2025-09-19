namespace Streamers.Features.Streamers.Dtos;

public class StreamerMeDto : StreamerDto
{
    public required bool FinishedAuth { get; set; }
    public required bool HasUnreadNotifications { get; set; }
}
