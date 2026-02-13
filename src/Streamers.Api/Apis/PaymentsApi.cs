using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using streamer.ServiceDefaults;
using Streamers.Features.Partners.Features.OnboardingCompleted;
using Streamers.Features.PaymentMethods.Features.AttachePaymentMethod;
using Streamers.Features.PaymentMethods.Features.DetachePaymentMethod;
using Streamers.Features.Subscriptions.Features.HandleSubscriptionCanceled;
using Streamers.Features.Subscriptions.Features.HandleSubscriptionInvoicePaid;
using Streamers.Features.Subscriptions.Features.HandleSubscriptionPastDue;
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
            else if (stripeEvent.Type == EventTypes.CustomerSubscriptionUpdated)
            {
                var subscription = stripeEvent.Data.Object as Subscription;
                var previous = stripeEvent.Data.PreviousAttributes as Newtonsoft.Json.Linq.JObject;

                if (subscription == null)
                    return TypedResults.Ok();

                var periodEnd = subscription.Items.Data.FirstOrDefault()?.CurrentPeriodEnd;
                if (periodEnd.HasValue && subscription.Status == "active")
                {
                    bool isStatusChanged = previous != null && previous.ContainsKey("status");
                    bool isPeriodChanged =
                        previous != null
                        && (
                            previous.ContainsKey("current_period_end")
                            || previous.ContainsKey("period_end")
                        );

                    if (isStatusChanged || isPeriodChanged || previous == null)
                    {
                        await mediator.Send(
                            new HandleSubscriptionInvoicePaid(subscription.Id, periodEnd.Value)
                        );
                    }
                }

                if (previous != null && previous.ContainsKey("status"))
                {
                    var oldStatus = previous["status"]?.ToString();
                    var newStatus = subscription.Status;

                    if (newStatus == "past_due")
                    {
                        await mediator.Send(new HandleSubscriptionPastDue(subscription.Id));
                    }
                }
            }
            else if (stripeEvent.Type == EventTypes.CustomerSubscriptionDeleted)
            {
                var subscription = stripeEvent.Data.Object as Subscription;
                if (subscription is not null)
                {
                    await mediator.Send(new HandleSubscriptionCanceled(subscription.Id));
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
