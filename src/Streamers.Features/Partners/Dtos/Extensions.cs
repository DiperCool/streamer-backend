using Streamers.Features.Partners.Models;

namespace Streamers.Features.Partners.Dtos;

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
