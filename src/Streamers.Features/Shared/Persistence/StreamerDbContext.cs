using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shared.Abstractions.Domain;
using Streamers.Features.Profiles.Models;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Shared.Persistence;

public class StreamerDbContext(
    DbContextOptions<StreamerDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher
) : DbContext(options)
{
    public DbSet<Streamer> Streamers { get; set; }
    public DbSet<Profile> Profiles { get; set; }

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
        int result = await base.SaveChangesAsync(cancellationToken);
        await PublishDomainEventsAsync();

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        await domainEventsDispatcher.DispatchAsync(domainEvents);
    }
}
