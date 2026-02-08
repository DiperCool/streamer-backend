namespace Shared.Stripe;

public class StripeOptions
{
    public const string Stripe = "Stripe";

    public string ApiKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}
