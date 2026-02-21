using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Subscriptions.Dtos;

namespace Streamers.Features.Subscriptions.Features.GetSubscriptionsByIds;

public record GetSubscriptionsByIds(IReadOnlyList<Guid> Ids)
    : IRequest<GetSubscriptionsByIdsResponse>;

public record GetSubscriptionsByIdsResponse(IDictionary<Guid, SubscriptionDto> Subscriptions);

public class GetSubscriptionsByIdsHandler(StreamerDbContext context)
    : IRequestHandler<GetSubscriptionsByIds, GetSubscriptionsByIdsResponse>
{
    public async Task<GetSubscriptionsByIdsResponse> Handle(
        GetSubscriptionsByIds request,
        CancellationToken cancellationToken
    )
    {
        var subscriptions = await context
            .Subscriptions.AsNoTracking()
            .Where(s => request.Ids.Contains(s.Id))
            .Select(s => new SubscriptionDto
            {
                Id = s.Id,
                UserId = s.UserId,
                StreamerId = s.StreamerId,
                Status = s.Status,
                CurrentPeriodEnd = s.CurrentPeriodEnd,
                CreatedAt = s.CreatedAt,
                Title = s.Title,
                CurrentStreak = s.CurrentStreak,
            })
            .ToDictionaryAsync(s => s.Id, cancellationToken);

        return new GetSubscriptionsByIdsResponse(subscriptions);
    }
}
