using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Abstractions.Domain;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SubscriptionPlans.Dtos;
using Streamers.Features.SubscriptionPlans.Models;

namespace Streamers.Features.SubscriptionPlans.Features.GetSubscriptionPlansByStreamerId;

public record GetSubscriptionPlansByStreamerId(string StreamerId)
    : IRequest<IEnumerable<SubscriptionPlanDto>>;

public class GetSubscriptionPlansByStreamerIdHandler(StreamerDbContext context)
    : IRequestHandler<GetSubscriptionPlansByStreamerId, IEnumerable<SubscriptionPlanDto>>
{
    public async Task<IEnumerable<SubscriptionPlanDto>> Handle(
        GetSubscriptionPlansByStreamerId request,
        CancellationToken cancellationToken
    )
    {
        var subscriptionPlans = await context
            .SubscriptionPlans.AsNoTracking()
            .Where(sp => sp.StreamerId == request.StreamerId)
            .Select(sp => new SubscriptionPlanDto
            {
                Id = sp.Id,
                StreamerId = sp.StreamerId,
                Name = sp.Name,
                Price = sp.Price,
            })
            .ToListAsync(cancellationToken);

        return subscriptionPlans;
    }
}
