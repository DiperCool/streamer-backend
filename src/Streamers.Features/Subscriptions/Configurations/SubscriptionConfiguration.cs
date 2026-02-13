using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streamers.Features.Subscriptions.Models;

namespace Streamers.Features.Subscriptions.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId).IsRequired().HasMaxLength(255);

        builder.Property(s => s.StreamerId).IsRequired().HasMaxLength(255);

        builder.Property(s => s.StripeSubscriptionId).IsRequired().HasMaxLength(255);
        builder.HasIndex(s => s.StripeSubscriptionId).IsUnique();

        builder.Property(s => s.Status).IsRequired().HasConversion<string>().HasMaxLength(50);

        builder.Property(s => s.CurrentPeriodEnd).IsRequired();

        builder.Property(s => s.CreatedAt).IsRequired();
        builder.HasQueryFilter(x => x.IsCurrent);
    }
}
