namespace Streamers.Features.Payments.Dtos;

public class PaymentMethodDto
{
    public required Guid Id { get; set; }
    public required string CardBrand { get; set; }
    public required string CardLast4 { get; set; }
    public required long CardExpMonth { get; set; }
    public required long CardExpYear { get; set; }
    public required bool IsDefault { get; set; }
}
