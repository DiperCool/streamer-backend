using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streamers.Features.Payouts.Models;

namespace Streamers.Features.Payouts.Configurations;

public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
{
    public void Configure(EntityTypeBuilder<Payout> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StreamerId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.StripePayoutId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ArrivalDate)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Streamer)
            .WithMany()
            .HasForeignKey(x => x.StreamerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
