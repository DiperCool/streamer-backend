using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Streamers.Features.Shared.GraphQl;

public static class Extensions
{
    public static IServiceCollection AddGraphQl(this IServiceCollection services)
    {
        services
            .AddGraphQL()
            .AddGraphQLServer()
            .AddAuthorization()
            .AddFeaturesTypes()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .AddSocketSessionInterceptor<AuthenticatedSocketSessionInterceptor>()
            .AddPagingArguments()
            .AddInMemorySubscriptions()
            .AddType<UploadType>()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ModifyCostOptions(o => o.EnforceCostLimits = false)
            .ModifyOptions(x => x.EnableFlagEnums = true);
        return services;
    }
}
