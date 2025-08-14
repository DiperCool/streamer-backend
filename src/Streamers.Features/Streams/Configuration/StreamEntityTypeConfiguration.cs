using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Streamers.Features.Streams.Configuration;

public class StreamEntityTypeConfiguration : IEntityTypeConfiguration<Models.Stream>
{
    public void Configure(EntityTypeBuilder<Models.Stream> builder)
    {
        builder.HasIndex(u => u.StreamId).IsUnique();
        builder
            .HasOne(x => x.Streamer)
            .WithMany(s => s.Streams)
            .HasForeignKey(x => x.StreamerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
