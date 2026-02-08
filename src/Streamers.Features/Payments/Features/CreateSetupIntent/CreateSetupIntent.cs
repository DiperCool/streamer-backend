using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Payments.Features.CreateSetupIntent;

public record CreateSetupIntent : IRequest<CreateSetupIntentResponse>;

public record CreateSetupIntentResponse(string ClientSecret);

public class CreateSetupIntentHandler(
    ICurrentUser currentUser,
    IStripeService stripeService,
    StreamerDbContext streamerDbContext
) : IRequestHandler<CreateSetupIntent, CreateSetupIntentResponse>
{
    public async Task<CreateSetupIntentResponse> Handle(
        CreateSetupIntent request,
        CancellationToken cancellationToken
    )
    {
        var streamer = await streamerDbContext
            .Streamers.Where(x => x.Id == currentUser.UserId)
            .Include(x => x.Customer)
            .FirstAsync(cancellationToken);

        var clientSecret = await stripeService.CreateSetupIntentAsync(
            streamer.Customer.StripeCustomerId ?? throw new InvalidOperationException("Stripe Error"),
            cancellationToken
        );

        return new CreateSetupIntentResponse(clientSecret);
    }
}
