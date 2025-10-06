using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shared.Abstractions.Domain;
using Streamers.Features.Analytics.Models;
using Streamers.Features.Banners.Model;
using Streamers.Features.Bots.Models;
using Streamers.Features.Categories.Models;
using Streamers.Features.Chats.Models;
using Streamers.Features.Followers.Models;
using Streamers.Features.Notifications.Models;
using Streamers.Features.Profiles.Models;
using Streamers.Features.Roles.Models;
using Streamers.Features.Settings.Models;
using Streamers.Features.Streamers.Models;
using Streamers.Features.StreamInfos.Models;
using Streamers.Features.Streams.Models;
using Streamers.Features.SystemRoles.Models;
using Streamers.Features.Tags.Models;
using Streamers.Features.Vods.Models;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.Shared.Persistance;

public class StreamerDbContext(
    DbContextOptions<StreamerDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher
) : DbContext(options)
{
    public DbSet<Streamer> Streamers { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Stream> Streams { get; set; }
    public DbSet<StreamSettings> StreamSettings { get; set; }
    public DbSet<StreamSource> StreamSources { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatSettings> ChatSettings { get; set; }
    public DbSet<PinnedChatMessage> PinnedChatMessages { get; set; }
    public DbSet<Follower> Followers { get; set; }
    public DbSet<Vod> Vods { get; set; }
    public DbSet<SystemRole> SystemRoles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<StreamInfo> StreamInfos { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<BannedUser> BannedUsers { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<LiveStartedNotification> LiveStartedNotifications { get; set; }
    public DbSet<VodSettings> VodSettings { get; set; }
    public DbSet<NotificationSettings> NotificationSettings { get; set; }
    public DbSet<UserFollowedNotification> UserFollowedNotifications { get; set; }
    public DbSet<AnalyticsItem> AnalyticsItems { get; set; }
    public DbSet<Bot> Bots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StreamerDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
        AddingVersioning(modelBuilder);
    }

    private static void AddingVersioning(ModelBuilder builder)
    {
        IEnumerable<IMutableEntityType> types = builder
            .Model.GetEntityTypes()
            .Where(x => x.ClrType.IsAssignableTo(typeof(IHaveAggregateVersion)));
        foreach (IMutableEntityType entityType in types)
        {
            builder
                .Entity(entityType.ClrType)
                .Property(nameof(IHaveAggregateVersion.OriginalVersion))
                .IsConcurrencyToken();
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<IHasDomainEvents>()
            .SelectMany(entity =>
            {
                var events = entity.Entity.DomainEvents;
                entity.Entity.ClearDomainEvents();
                return events;
            })
            .ToList();
        int result = await base.SaveChangesAsync(cancellationToken);
        await domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);

        return result;
    }
}
