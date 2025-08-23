namespace Streamers.Features.Chats.Dtos;

public class ChatSettingsDto
{
    public Guid Id { get; set; }
    public required int? SlowMode { get; set; }
    public required bool FollowersOnly { get; set; }
    public required bool SubscribersOnly { get; set; }
    public required List<string> BannedWords { get; set; }
}
