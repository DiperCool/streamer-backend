using System.Linq; // Added this using
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using streamer.ServiceDefaults;
using Streamers.Features.Partners.Features.OnboardingCompleted;
using Streamers.Features.PaymentMethods.Features.AttachePaymentMethod;
using Streamers.Features.PaymentMethods.Features.DetachePaymentMethod;
using Streamers.Features.Subscriptions.Features.CreateSubscription; // Added this using
using Stripe;

namespace Streamers.Api.Apis;

public static class PaymentsApi
{
    public static RouteGroupBuilder MapPaymentsApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("payments");

        api.MapPost("/stripe-webhook", HandleStripeWebhook);

        return api;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    private static async Task<Results<Ok, BadRequest<string>>> HandleStripeWebhook(
        HttpRequest request,
        IMediator mediator,
        IConfiguration configuration,
        ILogger<Event> logger
    )
    {
        var stripeOptions = configuration.BindOptions<StripeOptions>();
        var json = await new StreamReader(request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                request.Headers["Stripe-Signature"],
                stripeOptions.WebhookSecret
            );

            if (stripeEvent.Type == EventTypes.AccountUpdated)
            {
                var account = stripeEvent.Data.Object as Account;
                if (account?.DetailsSubmitted == true)
                {
                    await mediator.Send(new OnboardingCompletedCommand(account.Id));
                }
            }
            else if (stripeEvent.Type == EventTypes.PaymentMethodAttached)
            {
                var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                if (paymentMethod != null)
                {
                    await mediator.Send(
                        new PaymentMethodAttached(
                            paymentMethod.Id,
                            paymentMethod.CustomerId,
                            paymentMethod.Card.Brand,
                            paymentMethod.Card.Last4,
                            paymentMethod.Card.ExpMonth,
                            paymentMethod.Card.ExpYear
                        )
                    );
                }
            }
            else if (stripeEvent.Type == EventTypes.PaymentMethodDetached)
            {
                var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                if (paymentMethod != null)
                {
                    await mediator.Send(new PaymentMethodDetached(paymentMethod.Id));
                }
            }
            else if (stripeEvent.Type == EventTypes.CustomerSubscriptionCreated) // New condition
            {
                var stripeSubscription = stripeEvent.Data.Object as Subscription;
                if (stripeSubscription is not null)
                {
                    var userId = stripeSubscription.Metadata.TryGetValue("user_id", out var uid)
                        ? uid
                        : null;
                    var streamerId = stripeSubscription.Metadata.TryGetValue(
                        "streamer_id",
                        out var sid
                    )
                        ? sid
                        : null;

                    if (userId is not null && streamerId is not null)
                    {
                        await mediator.Send(
                            new CreateSubscription(
                                userId,
                                streamerId,
                                stripeSubscription.Id,
                                stripeSubscription.Items.Data.First().CurrentPeriodEnd
                            )
                        );
                    }
                }
            }

            return TypedResults.Ok();
        }
        catch (StripeException e)
        {
            logger.LogError(e, "Error processing stripe webhook");
            return TypedResults.BadRequest(e.Message);
        }
    }
}
