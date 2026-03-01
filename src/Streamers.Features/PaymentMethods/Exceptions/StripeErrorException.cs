namespace Streamers.Features.PaymentMethods.Exceptions;

public class StripeErrorException(string message) : Exception(message);