using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Enums;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Payments.Models;

public class Partner : Entity<Guid>
{
    public Streamer Streamer { get; private set; }

    public string StreamerId { get; private set; }
    public string? StripeAccountId { get; private set; }
    public StripeOnboardingStatus StripeOnboardingStatus { get; private set; } =
        StripeOnboardingStatus.NotStarted;

    public void StartOnboarding(string stripeAccountId)
    {
        StripeAccountId = stripeAccountId;
        StripeOnboardingStatus = StripeOnboardingStatus.InProgress;
    }

    public void CompleteOnboarding()
    {
        StripeOnboardingStatus = StripeOnboardingStatus.Completed;
    }
}
