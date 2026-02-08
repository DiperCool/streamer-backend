using Streamers.Features.Payments.Models;

namespace Streamers.Features.Payments.Dtos;

public static class Extensions
{
    public static PartnerDto ToPartnerDto(this Partner partner)
    {
        return new PartnerDto()
        {
            Id = partner.Id,
            StreamerId = partner.StreamerId,
            StripeAccountId = partner.StripeAccountId,
            StripeOnboardingStatus = partner.StripeOnboardingStatus
        };
    }
}
