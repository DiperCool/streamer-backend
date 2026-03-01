using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Subscriptions.Exceptions;

public class StripeCustomerNotFoundException() : NotFoundException("Stripe customer not found for the current user (payer).");