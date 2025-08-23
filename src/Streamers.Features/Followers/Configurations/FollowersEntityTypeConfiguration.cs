using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streamers.Features.Followers.Models;

namespace Streamers.Features.Followers.Configurations;

public class FollowersEntityTypeConfiguration : IEntityTypeConfiguration<Follower>
{
    public void Configure(EntityTypeBuilder<Follower> builder)
    {
        builder.HasIndex(f => new { f.FollowerStreamerId, f.StreamerId }).IsUnique();
    }
}
