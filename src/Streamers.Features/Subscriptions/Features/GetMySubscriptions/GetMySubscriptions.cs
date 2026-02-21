using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Subscriptions.Dtos;

namespace Streamers.Features.Subscriptions.Features.GetMySubscriptions;

public record GetMySubscriptions(
    QueryContext<SubscriptionDto> Query,
    PagingArguments PagingArguments
) : IRequest<Page<SubscriptionDto>>;

public class GetMySubscriptionsHandler(StreamerDbContext context, ICurrentUser currentUser)
    : IRequestHandler<GetMySubscriptions, Page<SubscriptionDto>>
{
    public async Task<Page<SubscriptionDto>> Handle(
        GetMySubscriptions request,
        CancellationToken cancellationToken
    )
    {
        var query = context.Subscriptions.Where(s => s.UserId == currentUser.UserId);

        var dtoQuery = query
            .Select(s => new SubscriptionDto
            {
                Id = s.Id,
                StreamerId = s.StreamerId,
                UserId = s.UserId,
                Status = s.Status,
                CurrentPeriodEnd = s.CurrentPeriodEnd,
                CreatedAt = s.CreatedAt,
                Title = s.Title,
                CurrentStreak = s.CurrentStreak,
            })
            .OrderByDescending(x => x.CreatedAt);
        Page<SubscriptionDto> result = await dtoQuery
            .With(request.Query)
            .ToPageAsync(request.PagingArguments, cancellationToken: cancellationToken);

        return result;
    }
}
