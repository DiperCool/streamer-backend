# Gemini Code Assistant Guidelines for the 'streamer' Backend

This document provides guidelines for using Gemini to assist with development in the `streamer` backend codebase. By following these conventions, the AI can generate code that is consistent with the existing style and architecture.

## 1. Project Overview

The `streamer` backend is a modern .NET 8 application built with a **Vertical Slice Architecture**. It serves a streaming platform, with **GraphQL (using Hot Chocolate) as its primary API**. It also exposes a few specific REST-like endpoints using Minimal APIs for internal webhooks and services.

- **Orchestration**: The project uses .NET Aspire for local development orchestration.
- **Core Technologies**:
  - ASP.NET Core
  - Entity Framework Core
  - GraphQL (Hot Chocolate)
  - CQRS with a Mediator pattern
  - Domain-Driven Design (DDD)
  - Hangfire for background jobs
  - RabbitMQ for messaging
  - Redis for caching

## 2. Architecture: Vertical Slices

The cornerstone of this project is its Vertical Slice Architecture, with features organized in the `Streamers.Features` project.

- **Rule**: When adding a new feature (e.g., "Add User Preference"), create a new folder for it inside the appropriate feature domain folder (e.g., `src/Streamers.Features/Profiles/Features/AddUserPreference/`).
- **Structure**: Each feature slice should be self-contained and include all necessary components: the CQRS command/query, the handler, DTOs, and any validation logic.

## 3. CQRS (Command Query Responsibility Segregation)

All business logic is implemented through CQRS commands and queries. The MediatR implementation used in this project is a custom one, located under `Shared.Abstractions.Cqrs`.

- **Commands/Queries**: Define commands and queries as `record` types within their feature slice file. They must implement `IRequest<TResponse>`. Always include a response record for commands and queries, even if it's an empty `Unit` response. Command and query record names should not include the words "Command" or "Query".
  ```csharp
  // Example: src/Streamers.Features/Streamers/Features/GetStreamer/GetStreamer.cs
  public record GetStreamer() : IRequest<StreamerMeDto>;
  public record CreateStreamer(string Name) : IRequest<CreateStreamerResponse>;
  public record CreateStreamerResponse(string Id);
  ```
- **Handlers**:
  - Implement handlers in the same file as their corresponding request.
  - The handler must implement `IRequestHandler<TRequest, TResponse>`.
  - Use C# 12 primary constructors for dependency injection.
  ```csharp
  // Example: src/Streamers.Features/Streamers/Features/GetStreamer/GetStreamer.cs
  public class GetStreamerHandler(StreamerDbContext context, ICurrentUser currentUser)
      : IRequestHandler<GetStreamer, StreamerMeDto>
  {
      public async Task<StreamerMeDto> Handle(GetStreamer request, CancellationToken cancellationToken)
      {
          // ... implementation
      }
  }
  ```

## 4. API Layer

The primary interface for this application is the GraphQL API. Minimal APIs should only be used for specific webhook implementations.

### GraphQL API (Primary)

- **Default Choice**: All new features and data exposure should be done through the GraphQL API.
- **Structure**: Follow the existing patterns for Queries, Mutations, and Types.
- **Location**: Place GraphQL-related classes (e.g., `MyFeatureQuery.cs`, `MyFeatureMutation.cs`) in a `Graphql` subfolder within the relevant feature directory (e.g., `src/Streamers.Features/Banners/Graphql/`).
- **Source Generation**: Leverage Hot Chocolate's source generation capabilities by using attributes for defining GraphQL types, queries, mutations, subscriptions, and data loaders. This reduces boilerplate and improves performance.

- **Mediator Usage**: All GraphQL query, mutation, and data loader methods *must* utilize the `IMediator` to dispatch a corresponding CQRS command or query. Direct database or service access within these methods is prohibited.

    ```csharp
    // Hot Chocolate GraphQL Query Type Definition
    [QueryType]
    public static partial class StreamerQuery
    {
        // Query definitions
        [Authorize]
        public static async Task<StreamerMeDto> GetMeAsync([Service] IMediator mediator)
        {
            var response = await mediator.Send(new GetStreamer());
            return response;
        }
    }

    // Hot Chocolate GraphQL Object Type Definition
    [ObjectType<StreamerDto>]
    public static partial class StreamerType
    {
        // Object type definitions
    }

    // Hot Chocolate GraphQL Subscription Type Definition
    [SubscriptionType]
    public static partial class StreamerSubscription
    {
        // Subscription definitions
        [Subscribe]
        [Topic($"{nameof(StreamerUpdated)}-{{{nameof(streamerId)}}}")]
        public static StreamerDto StreamerUpdated(
            string streamerId,
            [EventMessage] StreamerDto streamer
        ) => streamer;
    }

    // Hot Chocolate GraphQL Mutation Type Definition
    [MutationType]
    public static partial class StreamerMutation
    {
        // Mutation definitions
        [Authorize]
        public static async Task<UpdateAvatarResponse> UpdateAvatar(
            UpdateAvatar input,
            IMediator mediator
        )
        {
            return await mediator.Send(input);
        }
    }

    // Hot Chocolate GraphQL DataLoader Definition
    public static partial class StreamerDataLoader
    {
        [DataLoader]
        public static async Task<IDictionary<string, StreamerDto>> GetStreamersByIdAsync(
            IReadOnlyList<string> ids,
            [Service] IMediator mediator,
            CancellationToken cancellationToken
        )
        {
            var response = await mediator.Send(new GetStreamersByIds(ids), cancellationToken);
            return response.Streamers;
        }
    }
    
    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<StreamerDto>> GetStreamersAsync(
        string? search,
        [Service] IMediator mediator,
        QueryContext<StreamerDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetStreamers(search, rcontext, offsetPagingArguments),
            cancellationToken
        );
        return result.ToConnection();
    }
    public record GetStreamers(
    string? Search,
    QueryContext<StreamerDto> Query,
    PagingArguments PagingArguments
) : IRequest<Page<StreamerDto>>;

public class GetStreamersHandler(StreamerDbContext streamerDbContext)
: IRequestHandler<GetStreamers, Page<StreamerDto>>
{
public async Task<Page<StreamerDto>> Handle(
GetStreamers request,
CancellationToken cancellationToken
)
{
var query = streamerDbContext.Streamers.AsNoTracking();

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x => EF.Functions.ILike(x.UserName!, $"%{request.Search}%"));
        }

        var dtoQuery = query.Select(x => new StreamerDto
        {
            Id = x.Id,
            UserName = x.UserName,
            Avatar = x.Avatar,
            Followers = x.Followers,
            IsLive = x.IsLive,
        });

        Page<StreamerDto> result = await dtoQuery
            .With(request.Query)
            .ToPageAsync(request.PagingArguments, cancellationToken: cancellationToken);

        return result;
    }
}


    ```

### Minimal APIs (for Webhooks/Internal Services)

- **Usage**: Use Minimal APIs only for specific, non-primary endpoints, such as handling incoming webhooks (e.g., `/rtmp/checkToken`). Do not use them for standard CRUD operations or client-facing features.
- **Grouping**: Group related endpoints in a static class using an extension method on `IEndpointRouteBuilder`.
- **Typed Results**: When implementing a Minimal API endpoint, use `TypedResults` to return strongly-typed responses.

```csharp
// Example: An endpoint for an internal service or webhook
public static class StreamersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders").HasApiVersion(1.0);
        api.MapPut("/test", GetOrderAsync);
        return api;
    }

    public static Task<Results<Ok<string>, NotFound>> GetOrderAsync()
    {
        return Task.FromResult<Results<Ok<string>, NotFound>>(TypedResults.Ok("s"));
    }
}
```

## 5. Domain-Driven Design (DDD)

- **Entities**: Domain models are "rich" and contain business logic. They should inherit from the `Shared.Abstractions.Domain.Entity<TId>` base class.
- **Encapsulation**: Do not expose public setters for properties that should be controlled by business logic. Instead, create methods on the entity to modify its state (e.g., `streamer.SetLive(...)`).
- **Domain Events**: For significant state changes, raise domain events using the `Raise()` method within the entity. Define the event record in the same file as the entity that raises it.

```csharp
// Example: src/Streamers.Features/Streamers/Models/Streamer.cs
public record StreamerUpdated(Streamer Streamer) : IDomainEvent;

public class Streamer : Entity<string>
{
    public bool IsLive { get; private set; }
    // ... other properties

    public void SetLive(bool live, Stream? currentStream)
    {
        IsLive = live;
        // ... other logic
        Raise(new StreamerUpdated(this));
    }
}
```

## 6. C# Coding Style and Conventions

Adhere strictly to the modern C# style used throughout the project.

- **C# 12 Features**: Use primary constructors, file-scoped namespaces, records, and top-level statements.
- **Asynchrony**: Use `async`/`await` for all I/O-bound operations (database, HTTP calls, etc.). Suffix all async methods with `Async`.
- **Dependency Injection**: Always use constructor injection. For services and mediator handlers, prefer C# 12 primary constructors for dependency injection. Let the DI container manage service lifetimes.
- **Configuration Options**: When binding configuration to option classes (e.g., `Auth0Options`), use the `IConfiguration.BindOptions<T>()` extension method. This simplifies the process and eliminates the need for manual `services.Configure<T>()` and `IOptions<T>` injection in many cases. Example: `var options = configuration.BindOptions<Auth0Options>();`
- **Data Access**: Use Entity Framework Core for all database interactions.
- **Nullability**: Respect nullable reference types. Avoid `!` (null-forgiving operator).
- **Usings**: Keep `using` statements at the top of the file. `GlobalUsings.cs` is used for project-wide usings.
- **Comments**: Generally, avoid adding comments to the code. The code should be self-documenting. The only exception is for `TODO` comments, which should be used sparingly to mark incomplete tasks or future improvements that are absolutely necessary to track within the codebase. Do not edit comments that are separate from the code you are changing. *NEVER* talk to the user or describe your changes through comments.
- **NuGet Packages**: All NuGet packages must be managed using Central Package Management (CPM) via `Directory.Packages.props`. Versions are defined in `Directory.Packages.props`, and `.csproj` files should only contain package names without versions.
- **DTOs**: All properties in a Data Transfer Object (DTO) should be declared with the `required` keyword to enforce explicit initialization.
- **Third-Party Integrations**: All interactions with third-party services (e.g., Stripe, AWS S3) must be encapsulated within the `Shared` project. Organize these integrations into subfolders named after the library or service (e.g., `Shared/Stripe`, `Shared/Auth0`), utilizing appropriate abstractions (interfaces) to maintain separation of concerns and enable testability.

By following these guidelines, you can help ensure that all contributions, whether from a human or an AI, maintain the high quality and consistency of the `streamer` codebase.