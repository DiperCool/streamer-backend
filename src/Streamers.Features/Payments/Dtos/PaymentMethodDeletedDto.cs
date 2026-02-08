namespace Streamers.Features.Payments.Dtos;

public class PaymentMethodDeletedDto
{
    public required Guid PaymentMethodId { get; set; }
    public required string StreamerId { get; set; }
}
