using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Features.CreateCategory;
using Streamers.Features.Categories.Features.EditCategory;
using Streamers.Features.Categories.Features.RemoveCategory;

namespace Streamers.Features.Categories.Graphql;

[MutationType]
public static partial class CategoryMutations
{
    [Authorize]
    public static async Task<CreateCategoryResponse> CreateCategory(
        CreateCategory input,
        IMediator mediator
    )
    {
        return await mediator.Send(input);
    }

    [Authorize]
    public static async Task<EditCategoryResponse> UpdateCategory(
        EditCategory input,
        IMediator mediator
    )
    {
        return await mediator.Send(input);
    }

    [Authorize]
    public static async Task<RemoveCategoryResponse> RemoveCategory(
        RemoveCategory input,
        IMediator mediator
    )
    {
        return await mediator.Send(input);
    }
}
