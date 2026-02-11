using Stripe;

namespace Shared.Stripe;

public record CreateStripeAccountResult(string AccountId);
public record StripePaymentIntentResponse(string ClientSecret); // Added this record

public interface IStripeService
{
    Task<CreateStripeAccountResult> CreateExpressAccountAsync(
        string email,
        string userId,
        CancellationToken cancellationToken = default
    );
    Task<string> CreateCustomerAsync(
        string customerId,
        string email,
        CancellationToken cancellationToken
    );
    Task<Product?> GetProductByNameAsync(string name, CancellationToken cancellationToken);
    Task<Product> CreateProductAsync(
        string name,
        long unitAmount,
        string currency,
        CancellationToken cancellationToken
    );
    Task<(string OnboardingUrl, DateTime ExpiresAt)> CreateAccountLinkAsync(
        string stripeAccountId,
        CancellationToken cancellationToken
    );

    Task<string> CreateSetupIntentAsync(string customerId, CancellationToken cancellationToken);
    Task<bool> DetachPaymentMethodAsync(
        string customerId,
        string paymentMethodId,
        CancellationToken cancellationToken
    );

    Task<bool> UpdateCustomerDefaultPaymentMethodAsync(
        string customerId,
        string paymentMethodId,
        CancellationToken cancellationToken
    );
    Task<StripePaymentIntentResponse> CreatePaymentIntentAsync(
        long amount,
        string currency,
        string customerId,
        string? paymentMethodId,
        string? destinationAccountId, // Added destinationAccountId
        long? applicationFeeAmount, // Added applicationFeeAmount
        CancellationToken cancellationToken
    );
}
