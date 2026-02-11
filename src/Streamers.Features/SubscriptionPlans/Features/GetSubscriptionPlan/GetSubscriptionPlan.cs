using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Abstractions.Domain;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SubscriptionPlans.Dtos;
using Streamers.Features.SubscriptionPlans.Models;

namespace Streamers.Features.SubscriptionPlans.Features.GetSubscriptionPlan;

public record GetSubscriptionPlan(Guid Id) : IRequest<SubscriptionPlanDto>;

public class GetSubscriptionPlanHandler(StreamerDbContext context)
    : IRequestHandler<GetSubscriptionPlan, SubscriptionPlanDto>
{
    public async Task<SubscriptionPlanDto> Handle(
        GetSubscriptionPlan request,
        CancellationToken cancellationToken
    )
    {
        var subscriptionPlan = await context
            .SubscriptionPlans.AsNoTracking()
            .FirstOrDefaultAsync(sp => sp.Id == request.Id, cancellationToken);

        if (subscriptionPlan is null)
        {
            throw new InvalidOperationException(
                $"Subscription Plan with ID '{request.Id}' not found."
            );
        }

        return new SubscriptionPlanDto
        {
            Id = subscriptionPlan.Id,
            StreamerId = subscriptionPlan.StreamerId,
            Name = subscriptionPlan.Name,
            Price = subscriptionPlan.Price,
        };
    }
}
