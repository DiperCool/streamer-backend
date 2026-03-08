using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Streamers.Features.PaymentMethods.Models;
using Streamers.Features.SubscriptionPlans.Models;
using Streamers.Features.Subscriptions.Features.CreateSubscription;
using Streamers.Features.Subscriptions.Features.HandleSubscriptionInvoicePaid;
using Streamers.Features.Subscriptions.Models;
using CreateSubscriptionFeatureResponse = Streamers.Features.Subscriptions.Features.CreateSubscription.CreateSubscriptionResponse;
using CreateSubscriptionResponse = Shared.Stripe.CreateSubscriptionResponse;

namespace Streamers.Features.IntegrationTests.Subscriptions;

public class CreateSubscriptionTests : BaseIntegrationTest
{
    public CreateSubscriptionTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateSubscription_ShouldCreateSubscription()
    {
        // Arrange
        var subscribingStreamer = await CreateStreamer(Guid.NewGuid().ToString());
        var stripeCustomerId = "cus_subscriber";
        subscribingStreamer.Customer.MarkAsSuccess(stripeCustomerId);

        var partnerStreamer = await CreateStreamer(Guid.NewGuid().ToString());
        partnerStreamer.Partner.StartOnboarding("acct_partner");
        await DbContext.SaveChangesAsync();

        var subscriptionPlan = new SubscriptionPlan(
            partnerStreamer.Id,
            "prod_123",
            "price_123",
            "Test Plan",
            10m
        );

        var paymentMethod = new PaymentMethod(
            "pm_123",
            subscribingStreamer.Id,
            "visa",
            "4242",
            12,
            2030
        );

        await DbContext.SubscriptionPlans.AddAsync(subscriptionPlan);
        await DbContext.PaymentMethods.AddAsync(paymentMethod);
        await DbContext.SaveChangesAsync();

        CurrentUser.MakeAuthenticated(subscribingStreamer.Id);

        var clientSecret = "sub_client_secret";
        var subscriptionId = "sub_123";
        StripeService
            .CreateSubscriptionAsync(
                stripeCustomerId,
                subscriptionPlan.StripePriceId,
                paymentMethod.StripePaymentMethodId,
                partnerStreamer.Partner.StripeAccountId,
                Arg.Any<long>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<Dictionary<string, string>>()
            )
            .Returns(new CreateSubscriptionResponse(clientSecret, subscriptionId));

        var command = new CreateSubscription(subscriptionPlan.Id, paymentMethod.Id);

        // Act
        CreateSubscriptionFeatureResponse response = await Sender.Send(command);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(clientSecret, response.ClientSecret);

        var subscription = await DbContext
            .Subscriptions.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s =>
                s.UserId == subscribingStreamer.Id && s.StreamerId == partnerStreamer.Id
            );
        Assert.NotNull(subscription);
        Assert.Equal(subscriptionId, subscription.StripeSubscriptionId);

        // Arrange 2
        var invoicePaidCommand = new HandleSubscriptionInvoicePaid(
            subscriptionId,
            DateTime.UtcNow.AddMonths(1)
        );

        // Act 2
        await Sender.Send(invoicePaidCommand);

        // Assert 2
        await DbContext.Entry(subscription).ReloadAsync();
        Assert.Equal(SubscriptionStatus.Active, subscription.Status);
        Assert.True(subscription.IsCurrent);
    }
}
