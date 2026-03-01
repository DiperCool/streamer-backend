using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.PaymentMethods.Exceptions;

public class PaymentMethodNotFoundException() : NotFoundException("Payment method not found.");