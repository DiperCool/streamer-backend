using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Models;
using Streamers.Features.Notifications.Models;
using Streamers.Features.Payments.Models;
using Streamers.Features.Profiles.Models;
using Streamers.Features.Settings.Models;
using Streamers.Features.StreamInfos.Models;
using Streamers.Features.Streams.Models;
using Streamers.Features.SubscriptionPlans.Models;
using Streamers.Features.SystemRoles.Models;
using Streamers.Features.Vods.Models;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.Streamers.Models;

public record StreamerUpdated(Streamer Streamer) : IDomainEvent;

public record StreamerCreated(Streamer Streamer) : IDomainEvent;

public class Streamer : Entity<string>
{
    public string? UserName { get; set; }
    public bool IsLive { get; private set; }
    public string? Avatar { get; set; }
    public string Email { get; set; }
    public bool FinishedAuth { get; set; }
    public long Followers { get; set; }
    public Setting Setting { get; set; }
    public Profile Profile { get; set; }
    public StreamSettings StreamSettings { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Stream> Streams { get; set; } = new List<Stream>();
    public Chat Chat { get; set; }
    public ChatSettings ChatSettings { get; set; }
    public Guid? CurrentStreamId { get; set; }
    public Stream? CurrentStream { get; set; }
    public List<SystemRole> SystemRoles { get; set; }
    public StreamInfo StreamInfo { get; set; }
    public bool HasUnreadNotifications { get; set; }
    public NotificationSettings NotificationSettings { get; set; }
    public bool SubscriptionEnabled { get; private set; }
    public List<SubscriptionPlan> SubscriptionPlans { get; set; } = new();
    public Customer Customer { get; set; }
    public Partner Partner { get; set; }
    public List<PaymentMethod> PaymentMethods { get; set; } = new();

    private Streamer() { }

    public Streamer(
        string id,
        string userName,
        string email,
        Profile profile,
        Setting setting,
        StreamSettings streamSettings,
        Chat chat,
        DateTime createdAt,
        string? avatar,
        ChatSettings chatSettings,
        StreamInfo streamInfo,
        NotificationSettings notificationSettings,
        VodSettings vodSettings,
        Partner partner,
        Customer customer
    )
    {
        Id = id;
        UserName = userName;
        Email = email;
        Profile = profile;
        Setting = setting;
        CreatedAt = createdAt;
        Avatar = avatar;
        ChatSettings = chatSettings;
        StreamInfo = streamInfo;
        Chat = chat;
        StreamSettings = streamSettings;
        NotificationSettings = notificationSettings;
        VodSettings = vodSettings;
        Partner = partner;
        Customer = customer;
        Raise(new StreamerCreated(this));
    }

    public VodSettings VodSettings { get; set; }

    public void SetLive(bool live, Stream? currentStream)
    {
        IsLive = live;
        CurrentStreamId = currentStream?.Id;
        CurrentStream = currentStream;
        Raise(new StreamerUpdated(this));
    }

    public void FinishAuth(string userName)
    {
        UserName = userName;
        FinishedAuth = true;
    }

    public void EnableSubscriptions()
    {
        SubscriptionEnabled = true;
    }
}
