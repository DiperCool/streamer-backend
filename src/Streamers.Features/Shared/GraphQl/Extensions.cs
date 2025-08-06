using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using Streamers.Features.Streamers.GraphqQl;

namespace Streamers.Features.Shared.GraphQl;

public static class Extensions
{
    public static IServiceCollection AddGraphQl(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddAuthorization()
            .AddFeaturesTypes()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .AddPagingArguments()
            .AddInMemorySubscriptions()
            .AddType<UploadType>()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true);
        return services;
    }
}
