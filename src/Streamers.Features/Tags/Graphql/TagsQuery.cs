using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Tags.Dto;
using Streamers.Features.Tags.Features.GetTags;

namespace Streamers.Features.Tags.Graphql;

[QueryType]
public static partial class TagsQuery
{
    [UsePaging(MaxPageSize = 10)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<TagDto>> GetTags(
        [Service] IMediator mediator,
        QueryContext<TagDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetTags(offsetPagingArguments, rcontext),
            cancellationToken
        );
        return result.ToConnection();
    }
}
