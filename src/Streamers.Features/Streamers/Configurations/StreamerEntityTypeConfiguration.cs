using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Streamers.Configurations;

public class StreamerEntityTypeConfiguration : IEntityTypeConfiguration<Streamer>
{
    public void Configure(EntityTypeBuilder<Streamer> builder)
    {
        builder.HasIndex(u => u.UserName).IsUnique();
        builder
            .HasOne(x => x.CurrentStream)
            .WithOne()
            .HasForeignKey<Streamer>(x => x.CurrentStreamId)
            .OnDelete(DeleteBehavior.SetNull);
        builder
            .HasMany(x => x.Streams)
            .WithOne(x => x.Streamer)
            .HasForeignKey(x => x.StreamerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
