using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using streamer.ServiceDefaults;
using Streamers.Features.Partners.Features.OnboardingCompleted;
using Streamers.Features.PaymentMethods.Features.AttachePaymentMethod;
using Streamers.Features.PaymentMethods.Features.DetachePaymentMethod;
using Streamers.Features.Payouts.Features.HandlePayoutCreated;
using Streamers.Features.Payouts.Features.HandlePayoutFailed;
using Streamers.Features.Payouts.Features.HandlePayoutPaid;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Subscriptions.Features.HandleSubscriptionCanceled;
using Streamers.Features.Subscriptions.Features.HandleSubscriptionInvoicePaid;
using Streamers.Features.Subscriptions.Features.HandleSubscriptionPastDue;
using Streamers.Features.Transactions.Features.CreateTransaction;
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
            else if (stripeEvent.Type == EventTypes.InvoicePaid)
            {
                var invoice = stripeEvent.Data.Object as Invoice;
                if (invoice == null)
                {
                    logger.LogWarning("Invoice object is null for InvoicePaid event.");
                    return TypedResults.Ok();
                }

                if (
                    !invoice.Parent.SubscriptionDetails.Metadata.TryGetValue(
                        "payerId",
                        out var payerId
                    )
                    || !invoice.Parent.SubscriptionDetails.Metadata.TryGetValue(
                        "streamerId",
                        out var streamerId
                    )
                )
                {
                    logger.LogWarning(
                        "PayerId or StreamerId not found in subscription metadata for InvoicePaid event."
                    );
                    return TypedResults.Ok();
                }

                var grossAmount = invoice.AmountPaid / 100m;

                await mediator.Send(
                    new CreateTransaction(
                        payerId,
                        streamerId,
                        Features.Transactions.Models.TransactionType.Subscription,
                        grossAmount,
                        Features.Transactions.Models.TransactionStatus.Succeeded,
                        invoice.HostedInvoiceUrl
                    )
                );
            }
            else if (stripeEvent.Type == EventTypes.InvoicePaymentFailed)
            {
                var invoice = stripeEvent.Data.Object as Invoice;
                if (invoice == null)
                {
                    logger.LogWarning("Invoice object is null for InvoicePaymentFailed event.");
                    return TypedResults.Ok();
                }

                if (
                    !invoice.Parent.SubscriptionDetails.Metadata.TryGetValue(
                        "payerId",
                        out var payerId
                    )
                    || !invoice.Parent.SubscriptionDetails.Metadata.TryGetValue(
                        "streamerId",
                        out var streamerId
                    )
                )
                {
                    logger.LogWarning(
                        "PayerId or StreamerId not found in subscription metadata for InvoicePaymentFailed event."
                    );
                    return TypedResults.Ok();
                }

                var grossAmount = invoice.AmountDue / 100m;

                await mediator.Send(
                    new CreateTransaction(
                        payerId,
                        streamerId,
                        Features.Transactions.Models.TransactionType.Subscription,
                        grossAmount,
                        Features.Transactions.Models.TransactionStatus.Failed,
                        invoice.HostedInvoiceUrl
                    )
                );
            }
            else if (stripeEvent.Type == EventTypes.PayoutPaid)
            {
                var payout = stripeEvent.Data.Object as Payout;
                if (payout is not null)
                {
                    await mediator.Send(new HandlePayoutPaid(payout.Id));
                }
            }
            else if (stripeEvent.Type == EventTypes.PayoutFailed)
            {
                var payout = stripeEvent.Data.Object as Payout;
                if (payout is not null)
                {
                    await mediator.Send(
                        new HandlePayoutFailed(
                            payout.Id,
                            payout.FailureMessage ?? "Unknown failure"
                        )
                    );
                }
            }
            else if (stripeEvent.Type == EventTypes.PayoutCreated)
            {
                string stripeAccountId = stripeEvent.Account;
                var payout = stripeEvent.Data.Object as Payout;

                if (payout is not null)
                {
                    if (string.IsNullOrEmpty(stripeAccountId))
                    {
                        logger.LogWarning(
                            "Stripe Account ID is null or empty for PayoutCreated event. Cannot link to streamer."
                        );
                        return TypedResults.Ok();
                    }

                    await mediator.Send(
                        new HandlePayoutCreated(
                            payout.Id,
                            stripeAccountId,
                            payout.Amount / 100m,
                            payout.Currency,
                            payout.ArrivalDate,
                            payout.Status,
                            (payout.ApplicationFeeAmount ?? 0) / 100m
                        )
                    );
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
