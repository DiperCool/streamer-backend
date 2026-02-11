using Microsoft.Extensions.Configuration;
using streamer.ServiceDefaults;
using Stripe;

namespace Shared.Stripe;

public class StripeService(IConfiguration configuration) : IStripeService
{
    public async Task<CreateStripeAccountResult> CreateExpressAccountAsync(
        string email,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        var options = new AccountCreateOptions
        {
            Type = "express",
            Country = "US",
            Email = email,
            Capabilities = new AccountCapabilitiesOptions
            {
                CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
            },
            Metadata = new Dictionary<string, string> { { "userId", userId } },
        };
        var service = new AccountService();
        var account = await service.CreateAsync(options, cancellationToken: cancellationToken);
        var stripeOptions = configuration.BindOptions<StripeOptions>();
        var accountLinkOptions = new AccountLinkCreateOptions
        {
            Account = account.Id,
            Type = "account_onboarding",
        };

        return new CreateStripeAccountResult(account.Id);
    }

    public async Task<string> CreateCustomerAsync(
        string customerId,
        string email,
        CancellationToken cancellationToken
    )
    {
        var options = new CustomerCreateOptions
        {
            Email = email,
            Metadata = new Dictionary<string, string> { { "streamerId", customerId } },
        };
        var service = new CustomerService();
        var customer = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return customer.Id;
    }

    public async Task<Product?> GetProductByNameAsync(
        string name,
        CancellationToken cancellationToken
    )
    {
        var options = new ProductListOptions { Active = true };
        var service = new ProductService();
        var products = await service.ListAsync(options, cancellationToken: cancellationToken);
        return products.FirstOrDefault(p => p.Name == name);
    }

    public async Task<Product> CreateProductAsync(
        string name,
        long unitAmount,
        string currency,
        CancellationToken cancellationToken
    )
    {
        var productOptions = new ProductCreateOptions
        {
            Name = name,
            DefaultPriceData = new ProductDefaultPriceDataOptions
            {
                UnitAmount = unitAmount,
                Currency = currency,
                Recurring = new ProductDefaultPriceDataRecurringOptions { Interval = "month" },
            },
        };
        var productService = new ProductService();
        var product = await productService.CreateAsync(
            productOptions,
            cancellationToken: cancellationToken
        );
        return product;
    }

    public async Task<(string OnboardingUrl, DateTime ExpiresAt)> CreateAccountLinkAsync(
        string stripeAccountId,
        CancellationToken cancellationToken
    )
    {
        var accountLinkOptions = new AccountLinkCreateOptions
        {
            Account = stripeAccountId,
            Type = "account_onboarding",
            ReturnUrl = "http://localhost:5173/onboarding-completed", // This should be configurable
            RefreshUrl = "http://localhost:5173/onboarding-refresh", // This should be configurable
        };
        var accountLinkService = new AccountLinkService();
        var accountLink = await accountLinkService.CreateAsync(
            accountLinkOptions,
            cancellationToken: cancellationToken
        );
        return (accountLink.Url, accountLink.ExpiresAt);
    }

    public async Task<string> CreateSetupIntentAsync(
        string customerId,
        CancellationToken cancellationToken
    )
    {
        var options = new SetupIntentCreateOptions { Customer = customerId };
        var service = new SetupIntentService();
        var setupIntent = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return setupIntent.ClientSecret;
    }

    public async Task<bool> DetachPaymentMethodAsync(
        string customerId,
        string paymentMethodId,
        CancellationToken cancellationToken
    )
    {
        var service = new PaymentMethodService();
        var paymentMethod = await service.DetachAsync(
            paymentMethodId,
            new PaymentMethodDetachOptions(),
            cancellationToken: cancellationToken
        );
        return paymentMethod.Id != null;
    }

    public async Task<bool> UpdateCustomerDefaultPaymentMethodAsync(
        string customerId,
        string paymentMethodId,
        CancellationToken cancellationToken
    )
    {
        var options = new CustomerUpdateOptions
        {
            InvoiceSettings = new CustomerInvoiceSettingsOptions
            {
                DefaultPaymentMethod = paymentMethodId,
            },
        };
        var service = new CustomerService();
        var customer = await service.UpdateAsync(
            customerId,
            options,
            cancellationToken: cancellationToken
        );
        return customer.InvoiceSettings.DefaultPaymentMethod.Id == paymentMethodId;
    }

    public async Task<StripePaymentIntentResponse> CreatePaymentIntentAsync(
        long amount,
        string currency,
        string customerId,
        string? paymentMethodId,
        string? destinationAccountId,
        long? applicationFeeAmount,
        CancellationToken cancellationToken
    )
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = currency,
            Customer = customerId,
            PaymentMethod = paymentMethodId,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
        };

        if (destinationAccountId is not null && applicationFeeAmount is not null)
        {
            options.TransferData = new PaymentIntentTransferDataOptions
            {
                Destination = destinationAccountId,
            };
            options.ApplicationFeeAmount = applicationFeeAmount;
        }

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return new StripePaymentIntentResponse(paymentIntent.ClientSecret);
    }
}
