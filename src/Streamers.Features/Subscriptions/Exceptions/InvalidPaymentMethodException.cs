namespace Streamers.Features.Subscriptions.Exceptions;

public class InvalidPaymentMethodException() : Exception("Provided payment method is not valid for the current user.");