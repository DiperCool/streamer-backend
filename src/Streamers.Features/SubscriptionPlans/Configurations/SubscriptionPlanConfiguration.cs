using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streamers.Features.SubscriptionPlans.Models;

namespace Streamers.Features.SubscriptionPlans.Configurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Price)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(s => s.Streamer)
            .WithMany()
            .HasForeignKey(s => s.StreamerId)
            .IsRequired();
    }
}
