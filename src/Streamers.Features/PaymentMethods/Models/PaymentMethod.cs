using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.PaymentMethods.Models;

public record PaymentMethodCreated(PaymentMethod PaymentMethod) : IDomainEvent;

public record PaymentMethodDeleted(Guid PaymentMethodId, string StreamerId) : IDomainEvent;

public class PaymentMethod : Entity<Guid>
{
    private PaymentMethod() { } // Private constructor for EF Core

    public PaymentMethod(
        string stripePaymentMethodId,
        string streamerId,
        string cardBrand,
        string cardLast4,
        long cardExpMonth,
        long cardExpYear
    )
    {
        StripePaymentMethodId = stripePaymentMethodId;
        StreamerId = streamerId;
        CardBrand = cardBrand;
        CardLast4 = cardLast4;
        CardExpMonth = cardExpMonth;
        CardExpYear = cardExpYear;
        Raise(new PaymentMethodCreated(this));
    }

    public void Delete()
    {
        Raise(new PaymentMethodDeleted(Id, StreamerId));
    }

    public string StripePaymentMethodId { get; private set; }
    public string StreamerId { get; private set; }
    public Streamer Streamer { get; private set; }
    public string CardBrand { get; private set; }
    public string CardLast4 { get; private set; }
    public long CardExpMonth { get; private set; }
    public long CardExpYear { get; private set; }
    public bool IsDefault { get; private set; }

    public void MakeDefault()
    {
        IsDefault = true;
    }
}
