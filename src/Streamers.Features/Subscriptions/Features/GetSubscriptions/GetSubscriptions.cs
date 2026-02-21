using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Subscriptions.Dtos;

namespace Streamers.Features.Subscriptions.Features.GetSubscriptions;

public record GetSubscriptions(
    string StreamerId,
    string? Search,
    QueryContext<SubscriptionDto> Query,
    PagingArguments PagingArguments
) : IRequest<Page<SubscriptionDto>>;

public class GetSubscriptionsHandler(
    StreamerDbContext context,
    ICurrentUser currentUser,
    IRoleService roleService
) : IRequestHandler<GetSubscriptions, Page<SubscriptionDto>>
{
    public async Task<Page<SubscriptionDto>> Handle(
        GetSubscriptions request,
        CancellationToken cancellationToken
    )
    {
        if (!await roleService.HasRole(request.StreamerId, currentUser.UserId, Permissions.Revenue))
        {
            throw new UnauthorizedAccessException();
        }

        var query = context.Subscriptions.Where(s => s.StreamerId == request.StreamerId);

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x => EF.Functions.ILike(x.User.UserName!, $"%{request.Search}%"));
        }

        var dtoQuery = query.Select(s => new SubscriptionDto
        {
            Id = s.Id,
            UserId = s.UserId,
            StreamerId = s.StreamerId,
            Status = s.Status,
            CurrentPeriodEnd = s.CurrentPeriodEnd,
            CreatedAt = s.CreatedAt,
            Title = s.Title,
            CurrentStreak = s.CurrentStreak,
        });

        Page<SubscriptionDto> result = await dtoQuery
            .With(request.Query)
            .ToPageAsync(request.PagingArguments, cancellationToken: cancellationToken);

        return result;
    }
}
